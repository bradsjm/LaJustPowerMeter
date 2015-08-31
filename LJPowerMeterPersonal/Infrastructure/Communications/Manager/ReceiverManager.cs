/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.EIDSS.Communications
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using LaJust.EIDSS.Communications.Entities;
    using LaJust.EIDSS.Communications.Hardware;
    using System.Reflection;

    /// <summary>
    /// Manages the receivers attached to the system. 
    /// You should only have one manager per application.
    /// </summary>
    public class ReceiverManager : IReceiverManager, IDisposable
    {
        #region Private Constants

        private const string CLASSNAME = "ReceiverManager";
        private const string USB_DEVICE_ID = @"USB\VID_10C4&PID_EA60";

        #endregion

        #region Private Fields

        /// <summary>
        /// Provides dynamic detection of virtual com port insert/removal events
        /// </summary>
        private VirtualComPortsFinder _virtualComPortsFinder;

        /// <summary>
        /// Internal collection of managed receiver objects created and destroyed
        /// when virtual COM ports are added/removed
        /// </summary>
        private Dictionary<string, IReceiver> _recievers = new Dictionary<string, IReceiver>();

        /// <summary>
        /// Starting court number, incremented for each receiver in the system
        /// </summary>
        private byte _courtNumber;

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when the device reports a hit received.
        /// </summary>
        public event EventHandler<DeviceEventData> StrikeDetected = delegate { };

        /// <summary>
        /// Occurs when the receiver key pad button is pressed.
        /// </summary>
        public event EventHandler<PanelButtonEventData> PanelButtonPressed = delegate { };

        /// <summary>
        /// Occurs when a device is registered.
        /// </summary>
        public event EventHandler<DeviceRegistrationEventData> DeviceRegistered = delegate { };

        /// <summary>
        /// Device status update event (okay, not responding, battery low etc.)
        /// </summary>
        public event EventHandler<DeviceStatusEventData> DeviceStatusUpdate = delegate { };

        /// <summary>
        /// Occurs when [receiver count changed].
        /// </summary>
        public event EventHandler<ReceiverCountEventArgs> ReceiverCountChanged = delegate { };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecvrManager"/> class.
        /// </summary>
        public ReceiverManager(byte courtNumber) 
        {
            CheckCallerAccess(Assembly.GetCallingAssembly());
            Trace.TraceInformation("{0}.Constructor", CLASSNAME);
            Trace.Indent();

            // Set the starting court number
            _courtNumber = courtNumber;

            _virtualComPortsFinder = new VirtualComPortsFinder(USB_DEVICE_ID);

            // Wire up the event handlers
            _virtualComPortsFinder.VirtualPortCreated += VirtualComPortsFinder_VirtualPortCreated;
            _virtualComPortsFinder.VirtualPortRemoved += VirtualComPortsFinder_VirtualPortRemoved;

            Start();

            Trace.Unindent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the receivers collection.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<IReceiver> GetReceivers()
        {
            lock (_recievers)
            {
                return _recievers.Values.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Return count of receivers.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            lock (_recievers)
            {
                return _recievers.Count;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts this instance to manage new receivers.
        /// </summary>
        /// <param name="courtNumber">The court number.</param>
        private void Start()
        {
            Trace.TraceInformation("{0}.Start: Starting monitoring CourtNumber={1}", CLASSNAME, _courtNumber);
            Trace.Indent();

            // Populate the receiver manager with any existing receivers
            foreach (VirtualComPortEventArgs port in _virtualComPortsFinder.GetPorts())
            {
                try
                {
                    IReceiver receiver = new Receiver(port.PNPDeviceID, port.DeviceID, _courtNumber++);
                    // Hook up event handlers
                    receiver.StrikeDetected += OnStrikeDetected;
                    receiver.PanelButtonPressed += OnPanelButtonPressed;
                    receiver.DeviceStatusUpdate += OnDeviceStatusUpdate;
                    receiver.DeviceRegistered += OnDeviceRegistered;
                    // Clear any existing registrations from receiver
                    receiver.ClearGameRegistrations();
                    _recievers.Add(port.PNPDeviceID, receiver);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}.Start: Exception={1}", CLASSNAME, ex.GetBaseException());
                }
            }

            _virtualComPortsFinder.StartMonitoring(VirtualComPortsFinderMode.Both);
            OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = _recievers.Count });
            Trace.Unindent();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        private void Stop()
        {
            Trace.TraceInformation("{0}.Stop: Stopping monitoring for receivers", CLASSNAME);
            Trace.Indent();
            _virtualComPortsFinder.StopMonitoring();
            Trace.Unindent();
        }

        /// <summary>
        /// Handles the VirtualPortCreated event of the _virtualComPortsFinder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.VirtualComPortEventArgs"/> instance containing the event data.</param>
        private void VirtualComPortsFinder_VirtualPortCreated(object sender, VirtualComPortEventArgs e)
        {
            Trace.TraceInformation("{0}.VirtualPortCreated: PNPDeviceID={1} DeviceId={2} Caption={3}", CLASSNAME, e.PNPDeviceID, e.DeviceID, e.Caption);
            Trace.Indent();
            IReceiver receiver = new Receiver(e.PNPDeviceID, e.DeviceID, _courtNumber++);
            int count;

            // Hook up event handlers
            receiver.StrikeDetected += OnStrikeDetected;
            receiver.PanelButtonPressed += OnPanelButtonPressed;
            receiver.DeviceStatusUpdate += OnDeviceStatusUpdate;
            receiver.DeviceRegistered += OnDeviceRegistered;

            // Clear any existing registrations from receiver
            receiver.ClearGameRegistrations();

            lock (_recievers)
            {
                _recievers.Add(e.PNPDeviceID, receiver);
                count = _recievers.Count;
            }

            OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = count });
            Trace.Unindent();
        }

        /// <summary>
        /// Handles the VirtualPortRemoved event of the _virtualComPortsFinder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.VirtualComPortEventArgs"/> instance containing the event data.</param>
        private void VirtualComPortsFinder_VirtualPortRemoved(object sender, VirtualComPortEventArgs e)
        {
            Trace.TraceInformation("{0}.VirtualPortRemoved: PNPDeviceID={1}", CLASSNAME, e.PNPDeviceID);
            Trace.Indent();

            IReceiver receiver = null;
            int count;

            lock (_recievers)
            {
                if (_recievers.ContainsKey(e.PNPDeviceID))
                {
                    receiver = _recievers[e.PNPDeviceID];
                    _recievers.Remove(e.PNPDeviceID);
                }
                count = _recievers.Count;
            }

            if (receiver != null)
            {
                // Clear out game registrations
                receiver.ClearGameRegistrations();

                // UnHook event handlers
                receiver.StrikeDetected -= OnStrikeDetected;
                receiver.PanelButtonPressed -= OnPanelButtonPressed;
                receiver.DeviceStatusUpdate -= OnDeviceStatusUpdate;
                receiver.DeviceRegistered -= OnDeviceRegistered;

                receiver.Dispose();
            }

            OnReceiverCountChanged(this, new ReceiverCountEventArgs() { Count = count });
            Trace.Unindent();
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Called when [receiver count changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Manager.ReceiverCountEventArgs"/> instance containing the event data.</param>
        protected virtual void OnReceiverCountChanged(object sender, ReceiverCountEventArgs e)
        {
            EventHandler<ReceiverCountEventArgs> handler = ReceiverCountChanged;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnReceiverCountChanged: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:HitReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.HitReceivedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStrikeDetected(object sender, DeviceEventData e)
        {
            EventHandler<DeviceEventData> handler = StrikeDetected;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnStrikeDetected: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BatteryLow"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.BatteryLowEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceStatusUpdate(object sender, DeviceStatusEventData e)
        {
            EventHandler<DeviceStatusEventData> handler = DeviceStatusUpdate;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDeviceStatusUpdate: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:DeviceRegistered"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.DeviceRegistrationEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceRegistered(object sender, DeviceRegistrationEventData e)
        {
            EventHandler<DeviceRegistrationEventData> handler = DeviceRegistered;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDeviceRegistered: {1}", CLASSNAME, ex.GetBaseException());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:KeyPadButton"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.KeyPadButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPanelButtonPressed(object sender, PanelButtonEventData e)
        {
            EventHandler<PanelButtonEventData> handler = PanelButtonPressed;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnPanelButtonPressed: {1}", CLASSNAME, ex.GetBaseException());
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
            Trace.Indent();
            if (disposing)
            {
                if (_virtualComPortsFinder != null)
                {
                    _virtualComPortsFinder.VirtualPortCreated -= VirtualComPortsFinder_VirtualPortCreated;
                    _virtualComPortsFinder.VirtualPortRemoved -= VirtualComPortsFinder_VirtualPortRemoved;
                    _virtualComPortsFinder.Dispose();
                }
                foreach (Receiver recvr in _recievers.Values)
                {
                    recvr.Dispose();
                }
            }
            Trace.Unindent();
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

        #region Static Methods

        /// <summary>
        /// Checks the caller access.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private static void CheckCallerAccess(Assembly assembly)
        {
            byte[] myPublicKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();
            byte[] callerPublicKey = assembly.GetName().GetPublicKey();
            if (!myPublicKey.SequenceEqual(callerPublicKey)) throw new System.Security.SecurityException();
        }

        #endregion
    }

    #region Receiver Count Event Args

    /// <summary>
    /// Event triggered when a receiver is detected added or removed
    /// </summary>
    public class ReceiverCountEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current count of receivers.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }
    }

    #endregion
}
