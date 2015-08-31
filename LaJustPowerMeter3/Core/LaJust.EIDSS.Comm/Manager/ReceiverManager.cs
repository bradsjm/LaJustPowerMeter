// <copyright file="ReceiverManager.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using LaJust.EIDSS.Communications.Entities;
    using LaJust.EIDSS.Communications.Hardware;

    /// <summary>
    /// Manages the receivers attached to the system. 
    /// You should only have one manager per application.
    /// </summary>
    public class ReceiverManager : IReceiverManager, IDisposable
    {
        #region Private Constants

        /// <summary>
        /// The Name of this class for debugging purposes
        /// </summary>
        private const string CLASSNAME = "ReceiverManager";

        /// <summary>
        /// The USB Device ID for the EIDSS Receiver
        /// </summary>
        private const string USBDEVICEID = @"USB\VID_10C4&PID_EA60";

        #endregion

        #region Private Fields

        /// <summary>
        /// Provides dynamic detection of virtual com port insert/removal events
        /// </summary>
        private VirtualComPortsFinder virtualComPortsFinder;

        /// <summary>
        /// Internal collection of managed receiver objects created and destroyed
        /// when virtual COM ports are added/removed
        /// </summary>
        private Dictionary<string, IReceiver> recievers = new Dictionary<string, IReceiver>();

        /// <summary>
        /// Starting court number, incremented for each receiver in the system
        /// </summary>
        private byte courtNumber;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverManager"/> class.
        /// </summary>
        /// <param name="courtNumber">The court number.</param>
        public ReceiverManager(byte courtNumber) 
        {
            CheckCallerAccess(Assembly.GetCallingAssembly());
            Trace.TraceInformation("{0}.Constructor", CLASSNAME);
            Trace.Indent();

            // Set the starting court number
            this.courtNumber = courtNumber;

            this.virtualComPortsFinder = new VirtualComPortsFinder(USBDEVICEID);

            // Wire up the event handlers
            this.virtualComPortsFinder.VirtualPortCreated += this.VirtualComPortsFinder_VirtualPortCreated;
            this.virtualComPortsFinder.VirtualPortRemoved += this.VirtualComPortsFinder_VirtualPortRemoved;

            this.Start();

            Trace.Unindent();
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ReceiverManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~ReceiverManager()
        {
            throw new InvalidOperationException("ReceiverManager Dispose method not called.");
        }
        #endif

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when the device reports a hit received.
        /// </summary>
        public event EventHandler<DeviceDataEventArgs> StrikeDetected = delegate { };

        /// <summary>
        /// Occurs when the receiver key pad button is pressed.
        /// </summary>
        public event EventHandler<PanelButtonEventData> PanelButtonPressed = delegate { };

        /// <summary>
        /// Occurs when a device is registered.
        /// </summary>
        public event EventHandler<DeviceRegistrationEventArgs> DeviceRegistered = delegate { };

        /// <summary>
        /// Device status update event (okay, not responding, battery low etc.)
        /// </summary>
        public event EventHandler<DeviceStatusEventArgs> DeviceStatusUpdate = delegate { };

        /// <summary>
        /// Occurs when [receiver count changed].
        /// </summary>
        public event EventHandler<ReceiverCountEventArgs> ReceiverCountChanged = delegate { };

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the receivers collection.
        /// </summary>
        /// <returns>A readonly collection of connected receivers</returns>
        public ReadOnlyCollection<IReceiver> GetReceivers()
        {
            lock (this.recievers)
            {
                return this.recievers.Values.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Return count of receivers.
        /// </summary>
        /// <returns>The count of connected receivers</returns>
        public int Count()
        {
            lock (this.recievers)
            {
                return this.recievers.Count;
            }
        }

        /// <summary>
        /// Resets all receivers.
        /// </summary>
        public void ResetAll()
        {
            lock (this.recievers)
            {
                this.recievers.Values.ToList().ForEach(r => r.ClearGameRegistrations());
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Event Raising Methods

        /// <summary>
        /// Called when [receiver count changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.ReceiverCountEventArgs"/> instance containing the event data.</param>
        protected virtual void OnReceiverCountChanged(object sender, ReceiverCountEventArgs e)
        {
            EventHandler<ReceiverCountEventArgs> handler = this.ReceiverCountChanged;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnReceiverCountChanged: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:HitReceived"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Entities.DeviceDataEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStrikeDetected(object sender, DeviceDataEventArgs e)
        {
            EventHandler<DeviceDataEventArgs> handler = this.StrikeDetected;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnStrikeDetected: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BatteryLow"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Entities.DeviceStatusEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceStatusUpdate(object sender, DeviceStatusEventArgs e)
        {
            EventHandler<DeviceStatusEventArgs> handler = this.DeviceStatusUpdate;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDeviceStatusUpdate: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DeviceRegistered"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Device Registration Event Data.</param>
        protected virtual void OnDeviceRegistered(object sender, DeviceRegistrationEventArgs e)
        {
            EventHandler<DeviceRegistrationEventArgs> handler = this.DeviceRegistered;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDeviceRegistered: {1}", CLASSNAME, ex.GetBaseException());
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:KeyPadButton"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Panel Button Event Data.</param>
        protected virtual void OnPanelButtonPressed(object sender, PanelButtonEventData e)
        {
            EventHandler<PanelButtonEventData> handler = this.PanelButtonPressed;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnPanelButtonPressed: {1}", CLASSNAME, ex.GetBaseException());
                throw;
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
            Trace.Indent();
            if (disposing)
            {
                // Free other state (managed objects).
                if (this.virtualComPortsFinder != null)
                {
                    this.virtualComPortsFinder.VirtualPortCreated -= this.VirtualComPortsFinder_VirtualPortCreated;
                    this.virtualComPortsFinder.VirtualPortRemoved -= this.VirtualComPortsFinder_VirtualPortRemoved;
                    this.virtualComPortsFinder.Dispose();
                }

                foreach (Receiver recvr in this.recievers.Values)
                {
                    recvr.Dispose();
                }
            }
            Trace.Unindent();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Checks the caller access.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private static void CheckCallerAccess(Assembly assembly)
        {
            byte[] publicKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();
            byte[] callerPublicKey = assembly.GetName().GetPublicKey();
            if (!publicKey.SequenceEqual(callerPublicKey))
            {
                throw new System.Security.SecurityException();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts this instance to manage new receivers.
        /// </summary>
        private void Start()
        {
            Trace.TraceInformation("{0}.Start: Starting monitoring CourtNumber={1}", CLASSNAME, this.courtNumber);
            Trace.Indent();

            // Populate the receiver manager with any existing receivers
            foreach (VirtualComPortEventArgs port in this.virtualComPortsFinder.GetPorts())
            {
                try
                {
                    IReceiver receiver = new Receiver(port.PnpDeviceId, port.DeviceID, this.courtNumber++);
                    receiver.StrikeDetected += this.OnStrikeDetected;
                    receiver.PanelButtonPressed += this.OnPanelButtonPressed;
                    receiver.DeviceStatusUpdate += this.OnDeviceStatusUpdate;
                    receiver.DeviceRegistered += this.OnDeviceRegistered;
                    this.recievers.Add(port.PnpDeviceId, receiver);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}.Start: Exception={1}", CLASSNAME, ex.GetBaseException());
                }
            }

            this.virtualComPortsFinder.StartMonitoring(VirtualComPortsFinderMode.Both);
            this.OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = this.recievers.Count });
            Trace.Unindent();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        private void Stop()
        {
            Trace.TraceInformation("{0}.Stop: Stopping monitoring for receivers", CLASSNAME);
            Trace.Indent();
            this.virtualComPortsFinder.StopMonitoring();
            Trace.Unindent();
        }

        /// <summary>
        /// Handles the VirtualPortCreated event of the this._virtualComPortsFinder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.VirtualComPortEventArgs"/> instance containing the event data.</param>
        private void VirtualComPortsFinder_VirtualPortCreated(object sender, VirtualComPortEventArgs e)
        {
            Trace.TraceInformation("{0}.VirtualPortCreated: PNPDeviceID={1} DeviceId={2} Caption={3}", CLASSNAME, e.PnpDeviceId, e.DeviceID, e.Caption);
            Trace.Indent();
            IReceiver receiver = new Receiver(e.PnpDeviceId, e.DeviceID, this.courtNumber++);
            int count;

            // Hook up event handlers
            receiver.StrikeDetected += this.OnStrikeDetected;
            receiver.PanelButtonPressed += this.OnPanelButtonPressed;
            receiver.DeviceStatusUpdate += this.OnDeviceStatusUpdate;
            receiver.DeviceRegistered += this.OnDeviceRegistered;

            lock (this.recievers)
            {
                this.recievers.Add(e.PnpDeviceId, receiver);
                count = this.recievers.Count;
            }

            this.OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = count });
            Trace.Unindent();
        }

        /// <summary>
        /// Handles the VirtualPortRemoved event of the this._virtualComPortsFinder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.VirtualComPortEventArgs"/> instance containing the event data.</param>
        private void VirtualComPortsFinder_VirtualPortRemoved(object sender, VirtualComPortEventArgs e)
        {
            Trace.TraceInformation("{0}.VirtualPortRemoved: PNPDeviceID={1}", CLASSNAME, e.PnpDeviceId);
            Trace.Indent();

            IReceiver receiver = null;
            int count;

            lock (this.recievers)
            {
                if (this.recievers.ContainsKey(e.PnpDeviceId))
                {
                    receiver = this.recievers[e.PnpDeviceId];
                    this.recievers.Remove(e.PnpDeviceId);
                }

                count = this.recievers.Count;
            }

            if (receiver != null)
            {
                // UnHook event handlers
                receiver.StrikeDetected -= this.OnStrikeDetected;
                receiver.PanelButtonPressed -= this.OnPanelButtonPressed;
                receiver.DeviceStatusUpdate -= this.OnDeviceStatusUpdate;
                receiver.DeviceRegistered -= this.OnDeviceRegistered;

                receiver.Dispose();
            }

            this.OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = count });
            Trace.Unindent();
        }

        #endregion
    }
}
