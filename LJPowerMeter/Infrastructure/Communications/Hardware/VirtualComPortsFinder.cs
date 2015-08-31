/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management;
    using System.Reflection;
    using System.Threading;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Virtual Comm Port Event Insert/Removal Arguments
    /// </summary>
    internal class VirtualComPortEventArgs : EventArgs
    {
        #region Public Properties
        public string Caption { get; internal set; }
        public string Description { get; internal set; }
        public string DeviceID { get; internal set; }
        public string Name { get; internal set; }
        public string PNPDeviceID { get; internal set; }
        #endregion
    }

    /// <summary>
    /// Monitor mode
    /// </summary>
    internal enum VirtualComPortsFinderMode
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
    internal class VirtualComPortsFinder : IDisposable
    {
        private const string CLASSNAME = "VirtualComPortsFinder";

        #region Private Fields

        private ManagementEventWatcher _usbInsertWatcher;
        private ManagementEventWatcher _usbRemovalWatcher;
        private string _pnpDeviceId;

        #endregion

        #region Public Properties

        /// <summary>  
        /// Gets or sets the driver 'instanceId' to look for.  
        /// </summary>  
        internal string PnpDeviceId
        {
            get { return _pnpDeviceId; }
            set { _pnpDeviceId = value; }
        }

        #endregion

        #region Internal Events

        /// <summary>
        /// Occurs when a new virtual port is created for the specified driver ID.
        /// </summary>
        internal event EventHandler<VirtualComPortEventArgs> VirtualPortCreated = delegate { };

        /// <summary>
        /// Occurs when a new virtual port is removed for the specified driver ID.
        /// </summary>
        internal event EventHandler<VirtualComPortEventArgs> VirtualPortRemoved = delegate { };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VcpFinder"/> class.
        /// You should call StartMonitoring() to actually generate events
        /// </summary>
        /// <param name="pnpDeviceId">The PNP device id.</param>
        public VirtualComPortsFinder(string pnpDeviceId)
        {
            Trace.TraceInformation("{0}.Constructor: pnpDeviceId={1}", CLASSNAME, pnpDeviceId);
            _pnpDeviceId = pnpDeviceId;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Returns a list of virtual com ports.
        /// </summary>
        /// <returns></returns>
        internal VirtualComPortEventArgs[] GetPorts()
        {
            List<VirtualComPortEventArgs> ports = new List<VirtualComPortEventArgs>();

            using (ManagementObjectSearcher results = new ManagementObjectSearcher("root\\CIMV2", "select * from Win32_SerialPort"))
            {
                foreach (ManagementBaseObject commPort in results.Get()) using (commPort)
                {
                    if (commPort["PNPDeviceID"].ToString().StartsWith(_pnpDeviceId))
                    {
                        ports.Add(CreateVirtualComPortEventArgs(commPort));
                    }
                }
            }
            return ports.ToArray();
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        internal void StartMonitoring(VirtualComPortsFinderMode monitorMode)
        {
            // Clear any existing monitoring if it exists
            StopMonitoring();

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
                    _usbInsertWatcher = new ManagementEventWatcher(scope, eventQuery);
                    _usbInsertWatcher.EventArrived += UsbInsertWatcher_EventArrived;
                    _usbInsertWatcher.Start();
                    Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort insert watcher started", CLASSNAME);
                }

                if (monitorMode == VirtualComPortsFinderMode.Removed || monitorMode == VirtualComPortsFinderMode.Both)
                {
                    // Subscribe to instance removal events
                    eventQuery = new WqlEventQuery("__InstanceDeletionEvent");
                    eventQuery.WithinInterval = new TimeSpan(0, 0, 3);
                    eventQuery.Condition = @"TargetInstance ISA 'Win32_SerialPort'";
                    _usbRemovalWatcher = new ManagementEventWatcher(scope, eventQuery);
                    _usbRemovalWatcher.EventArrived += UsbRemovalWatcher_EventArrived;
                    _usbRemovalWatcher.Start();
                    Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort removal watcher started", CLASSNAME);
                }
            }
            catch (Exception ex)
            {
                // Make sure we cleanup properly if there is an error
                Trace.TraceError("{0}.StartMonitoring: Exception={1}", CLASSNAME, ex.GetBaseException());
                StopMonitoring();
                throw;
            }
        }

        /// <summary>
        /// Stops the monitoring.
        /// </summary>
        internal void StopMonitoring()
        {
            if (_usbInsertWatcher != null)
            {
                _usbInsertWatcher.EventArrived -= UsbInsertWatcher_EventArrived;
                _usbInsertWatcher.Dispose();
                _usbInsertWatcher = null;
                Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort insert watcher disposed", CLASSNAME);
            }
            if (_usbRemovalWatcher != null)
            {
                _usbRemovalWatcher.EventArrived -= UsbRemovalWatcher_EventArrived;
                _usbRemovalWatcher.Dispose();
                _usbRemovalWatcher = null;
                Trace.TraceInformation("{0}.StartMonitoring: Win32_SerialPort removal watcher disposed", CLASSNAME);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Helper method that creates the virtual COM port event args.
        /// </summary>
        /// <param name="commObject">The comm object.</param>
        /// <returns>VirtualComPortEventArgs</returns>
        private VirtualComPortEventArgs CreateVirtualComPortEventArgs(ManagementBaseObject commObject)
        {
            return new VirtualComPortEventArgs()
            {
                Caption = commObject["Caption"].ToString(),
                Description = commObject["Description"].ToString(),
                DeviceID = commObject["DeviceID"].ToString(),
                Name = commObject["Name"].ToString(),
                PNPDeviceID = commObject["PNPDeviceID"].ToString()
            };
        }

        /// <summary>
        /// Handles the EventArrived event of the UsbInsertWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Management.EventArrivedEventArgs"/> instance containing the event data.</param>
        private void UsbInsertWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            using (ManagementBaseObject commPort = (ManagementBaseObject)(e.NewEvent["TargetInstance"]))
            {
                if (!commPort["PNPDeviceID"].ToString().StartsWith(_pnpDeviceId)) return;
                Trace.TraceInformation("{0}.UsbInsertWatcher_EventArrived: {1}", CLASSNAME, commPort["Name"].ToString());
                Thread.Sleep(TimeSpan.FromSeconds(5)); // Give the system enough time to process this event
                OnVirtualPortCreated(CreateVirtualComPortEventArgs(commPort));
            }
        }

        /// <summary>
        /// Handles the EventArrived event of the UsbRemovalWatcher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Management.EventArrivedEventArgs"/> instance containing the event data.</param>
        private void UsbRemovalWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            using (ManagementBaseObject commPort = (ManagementBaseObject)(e.NewEvent["TargetInstance"]))
            {
                if (!commPort["PNPDeviceID"].ToString().StartsWith(_pnpDeviceId)) return;
                Trace.TraceInformation("{0}.UsbRemovalWatcher_EventArrived: {1}", CLASSNAME, commPort["Name"].ToString());
                OnVirtualPortRemoved(CreateVirtualComPortEventArgs(commPort));
            }
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Raises the <see cref="E:VirtualPortCreated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.VirtualComPortEventArgs"/> instance containing the event data.</param>
        internal virtual void OnVirtualPortCreated(VirtualComPortEventArgs e)
        {
            EventHandler<VirtualComPortEventArgs> handler = VirtualPortCreated;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnVirtualPortCreated: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:VirtualPortRemoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.VirtualComPortEventArgs"/> instance containing the event data.</param>
        internal virtual void OnVirtualPortRemoved(VirtualComPortEventArgs e)
        {
            EventHandler<VirtualComPortEventArgs> handler = VirtualPortRemoved;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnVirtualPortRemoved: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            Trace.TraceInformation("{0}.Dispose: Disposing={1}", CLASSNAME, disposing);
            if (disposing)
            {
                if (_usbInsertWatcher != null)
                {
                    _usbInsertWatcher.EventArrived -= UsbInsertWatcher_EventArrived;
                    _usbInsertWatcher.Dispose();
                    _usbInsertWatcher = null;
                }
                if (_usbRemovalWatcher != null)
                {
                    _usbRemovalWatcher.EventArrived -= UsbRemovalWatcher_EventArrived;
                    _usbRemovalWatcher.Dispose();
                    _usbInsertWatcher = null;
                }
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
