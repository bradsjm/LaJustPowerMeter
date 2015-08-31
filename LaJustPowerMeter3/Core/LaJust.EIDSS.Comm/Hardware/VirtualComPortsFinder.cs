// <copyright file="VirtualComPortsFinder.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management;
    using System.Threading;

    /// <summary>
    /// Virtual Com Port Finder Monitor mode
    /// </summary>
    public enum VirtualComPortsFinderMode
    {
        #region Enumeration Values
        /// <summary>
        /// Do not monitor
        /// </summary>
        None,

        /// <summary>
        /// Only monitor for added ports
        /// </summary>
        Added,

        /// <summary>
        /// Only monitor for removed ports
        /// </summary>
        Removed,

        /// <summary>
        /// Monitor for both added and removed ports
        /// </summary>
        Both
        #endregion
    }

    /// <summary>  
    /// Methods to find a port name from a given device id  
    /// </summary>  
    public class VirtualComPortsFinder : IDisposable
    {
        /// <summary>
        /// Class Name for Trace Logging Purposes
        /// </summary>
        private const string CLASSNAME = "VirtualComPortsFinder";

        #region Private Fields

        /// <summary>
        /// USB Insert Watcher
        /// </summary>
        private ManagementEventWatcher usbInsertWatcher;

        /// <summary>
        /// USB Removal Watcher
        /// </summary>
        private ManagementEventWatcher usbRemovalWatcher;

        /// <summary>
        /// Plug-N-Play Device Id to monitor for
        /// </summary>
        private string pnpDeviceId;

        #endregion

        #region Public Constructors and Disposer

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualComPortsFinder"/> class.
        /// You should call StartMonitoring() to actually generate events
        /// </summary>
        /// <param name="pnpDeviceId">The PNP device id.</param>
        public VirtualComPortsFinder(string pnpDeviceId)
        {
            Trace.TraceInformation("{0}.Constructor: pnpDeviceId={1}", CLASSNAME, pnpDeviceId);
            this.pnpDeviceId = pnpDeviceId;
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Receiver"/> is reclaimed by garbage collection.
        /// </summary>
        ~VirtualComPortsFinder()
        {
            throw new InvalidOperationException("VirtualComPortsFinder Dispose method not called.");
        }
        #endif

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when a new virtual port is created for the specified driver ID.
        /// </summary>
        public event EventHandler<VirtualComPortEventArgs> VirtualPortCreated = delegate { };

        /// <summary>
        /// Occurs when a new virtual port is removed for the specified driver ID.
        /// </summary>
        public event EventHandler<VirtualComPortEventArgs> VirtualPortRemoved = delegate { };

        #endregion

        #region public Properties

        /// <summary>  
        /// Gets or sets the driver 'instanceId' to look for.  
        /// </summary>  
        public string PnpDeviceId
        {
            get { return this.pnpDeviceId; }
            set { this.pnpDeviceId = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a list of virtual com ports.
        /// </summary>
        /// <returns>List of virtual com ports</returns>
        public VirtualComPortEventArgs[] GetPorts()
        {
            List<VirtualComPortEventArgs> ports = new List<VirtualComPortEventArgs>();

            using (ManagementObjectSearcher results = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_SerialPort"))
            {
                foreach (ManagementBaseObject commPort in results.Get())
                {
                    using (commPort)
                    {
                        if (commPort["pnpDeviceId"].ToString().StartsWith(this.pnpDeviceId))
                        {
                            ports.Add(this.CreateVirtualComPortEventArgs(commPort));
                        }
                    }
                }
            }

            return ports.ToArray();
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        /// <param name="monitorMode">The monitor mode.</param>
        public void StartMonitoring(VirtualComPortsFinderMode monitorMode)
        {
            // Clear any existing monitoring if it exists
            this.StopMonitoring();

            // Add subscriptions to WMI insert and removal events
            WqlEventQuery eventQuery;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {
                if (monitorMode == VirtualComPortsFinderMode.Added || monitorMode == VirtualComPortsFinderMode.Both)
                {
                    // Subscribe to instance creation events
                    eventQuery = new WqlEventQuery("__InstanceCreationEvent");
                    eventQuery.WithinInterval = new TimeSpan(0, 0, 3);
                    eventQuery.Condition = @"TargetInstance ISA 'Win32_SerialPort'";
                    this.usbInsertWatcher = new ManagementEventWatcher(scope, eventQuery);
                    this.usbInsertWatcher.EventArrived += this.UsbInsertWatcher_EventArrived;
                    this.usbInsertWatcher.Start();
                    Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort insert watcher started", CLASSNAME);
                }

                if (monitorMode == VirtualComPortsFinderMode.Removed || monitorMode == VirtualComPortsFinderMode.Both)
                {
                    // Subscribe to instance removal events
                    eventQuery = new WqlEventQuery("__InstanceDeletionEvent");
                    eventQuery.WithinInterval = new TimeSpan(0, 0, 3);
                    eventQuery.Condition = @"TargetInstance ISA 'Win32_SerialPort'";
                    this.usbRemovalWatcher = new ManagementEventWatcher(scope, eventQuery);
                    this.usbRemovalWatcher.EventArrived += this.UsbRemovalWatcher_EventArrived;
                    this.usbRemovalWatcher.Start();
                    Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort removal watcher started", CLASSNAME);
                }
            }
            catch (Exception ex)
            {
                // Make sure we cleanup properly if there is an error
                Trace.TraceError("{0}.StartMonitoring: Exception={1}", CLASSNAME, ex.GetBaseException());
                this.StopMonitoring();
                throw;
            }
        }

        /// <summary>
        /// Stops the monitoring.
        /// </summary>
        public void StopMonitoring()
        {
            if (this.usbInsertWatcher != null)
            {
                this.usbInsertWatcher.Stop();
                this.usbInsertWatcher.EventArrived -= this.UsbInsertWatcher_EventArrived;
                this.usbInsertWatcher.Dispose();
                this.usbInsertWatcher = null;
                Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort insert watcher disposed", CLASSNAME);
            }

            if (this.usbRemovalWatcher != null)
            {
                this.usbRemovalWatcher.Stop();
                this.usbRemovalWatcher.EventArrived -= this.UsbRemovalWatcher_EventArrived;
                this.usbRemovalWatcher.Dispose();
                this.usbRemovalWatcher = null;
                Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort removal watcher disposed", CLASSNAME);
            }
        }

        #endregion

        #region Protected IDisposable Implementation

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            Trace.TraceInformation("{0}.Dispose: Disposing={1}", CLASSNAME, disposing);
            if (disposing)
            {
                if (this.usbInsertWatcher != null)
                {
                    this.usbInsertWatcher.EventArrived -= this.UsbInsertWatcher_EventArrived;
                    this.usbInsertWatcher.Dispose();
                    this.usbInsertWatcher = null;
                }

                if (this.usbRemovalWatcher != null)
                {
                    this.usbRemovalWatcher.EventArrived -= this.UsbRemovalWatcher_EventArrived;
                    this.usbRemovalWatcher.Dispose();
                    this.usbInsertWatcher = null;
                }
            }
        }

        #endregion

        #region Protected Event Raising Methods

        /// <summary>
        /// Raises the <see cref="E:VirtualPortCreated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.VirtualComPortEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVirtualPortCreated(VirtualComPortEventArgs e)
        {
            EventHandler<VirtualComPortEventArgs> handler = this.VirtualPortCreated;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnVirtualPortCreated: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:VirtualPortRemoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.VirtualComPortEventArgs"/> instance containing the event data.</param>
        protected virtual void OnVirtualPortRemoved(VirtualComPortEventArgs e)
        {
            EventHandler<VirtualComPortEventArgs> handler = this.VirtualPortRemoved;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnVirtualPortRemoved: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Helper method that creates the virtual COM port event args.
        /// </summary>
        /// <param name="commObject">The comm object.</param>
        /// <returns>Event Args for COM port management object</returns>
        private VirtualComPortEventArgs CreateVirtualComPortEventArgs(ManagementBaseObject commObject)
        {
            return new VirtualComPortEventArgs()
            {
                Caption = commObject["Caption"].ToString(),
                Description = commObject["Description"].ToString(),
                DeviceID = commObject["DeviceID"].ToString(),
                Name = commObject["Name"].ToString(),
                PnpDeviceId = commObject["pnpDeviceId"].ToString()
            };
        }

        /// <summary>
        /// Handles the EventArrived event of the this.usbInsertWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Management.EventArrivedEventArgs"/> instance containing the event data.</param>
        private void UsbInsertWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            using (ManagementBaseObject commPort = (ManagementBaseObject)e.NewEvent["TargetInstance"])
            {
                if (commPort["pnpDeviceId"].ToString().StartsWith(this.pnpDeviceId))
                {
                    Trace.TraceInformation("{0}.this.usbInsertWatcher_EventArrived: {1}", CLASSNAME, commPort["Name"].ToString());
                    Thread.Sleep(TimeSpan.FromSeconds(5)); // Give the system enough time to process this event
                    this.OnVirtualPortCreated(this.CreateVirtualComPortEventArgs(commPort));
                }
            }
        }

        /// <summary>
        /// Handles the EventArrived event of the this.usbRemovalWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Management.EventArrivedEventArgs"/> instance containing the event data.</param>
        private void UsbRemovalWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            using (ManagementBaseObject commPort = (ManagementBaseObject)e.NewEvent["TargetInstance"])
            {
                if (commPort["pnpDeviceId"].ToString().StartsWith(this.pnpDeviceId))
                {
                    Trace.TraceInformation("{0}.this.usbRemovalWatcher_EventArrived: {1}", CLASSNAME, commPort["Name"].ToString());
                    this.OnVirtualPortRemoved(this.CreateVirtualComPortEventArgs(commPort));
                }
            }
        }

        #endregion
    }
}
