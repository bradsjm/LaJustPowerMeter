/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

// Enable this for legacy EIDSS 2.0 support (original receiver)
#define EIDSS2

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using LaJust.EIDSS.Communications.Entities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Receiver Operation Class
    /// </summary>
    public class Receiver : IReceiver, IDisposable
    {
        #region Private Constants
        /// <summary>
        /// Class name for use in debug/trace logging
        /// </summary>
        private const string CLASSNAME = "Receiver";

        /// <summary>
        /// Maximum number of courts supported by the hardware (1 - 8)
        /// </summary>
        private const byte MAX_COURTS = 8;

        #endregion

        #region Private Fields

        /// <summary>
        /// The Registration Sequence number starting value is randomized based on clock ticks since boot.
        /// It is incremented for each registration on any receiver so we use a static member.
        /// </summary>
        private static byte _registrationSeq = (byte)(Environment.TickCount % 0xFF);
        /// <summary>
        /// Each registered device on this receiver is recorded in this dictionary.
        /// </summary>
        private Dictionary<DeviceId, DeviceRegistrationEventData> _deviceRegistrations = new Dictionary<DeviceId, DeviceRegistrationEventData>();
        /// <summary>
        /// Court number to use. Each receiver should be assigned a unique court number (from 1 - 8)
        /// </summary>
        private byte _courtNumber;
        /// <summary>
        /// The receiver serial communications object used to communicate to the
        /// device over USB.
        /// </summary>
        private ReceiverSerialComm _recvrComm;
        /// <summary>
        /// Each packet from the receiver contains a sequence number. We track this to detect and ignore any
        /// duplicate packets.
        /// </summary>
        private byte _lastSeqNumber = 0xFF;
        #if EIDSS2
        /// <summary>
        /// The original V1 receiver does not provide device ID information for each strike (V2 does). We keep
        /// track of the device Ids provided during registration so we can insert them in the strike packet.
        /// </summary>
        private DeviceId _ChungDeviceId = new DeviceId();
        private DeviceId _HongDeviceId = new DeviceId();
        #endif

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the identification value for this object.
        /// </summary>
        /// <value>The id.</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the court number (1-8) assigned to this receiver.
        /// </summary>
        /// <value>The court number.</value>
        public byte CourtNumber
        {
            get { return _courtNumber; }
            private set { _courtNumber = (byte)((value - 1 % MAX_COURTS) + 1); }  // Enforce court # must be from 1 through 8
        }

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver"/> class.
        /// </summary>
        /// <param name="Id">The id of this receiver</param>
        /// <param name="commPort">The recvr serial port (e.g. COM3)</param>
        /// <param name="courtNumber">The court number assigned to this receiver (e.g. 1)</param>
        public Receiver(string id, string commPort, byte courtNumber)
        {
            CheckCallerAccess(Assembly.GetCallingAssembly());
            Trace.TraceInformation("{0}.Constructor: Id={1} CommPort={2} CourtNumber={3}", CLASSNAME, id, commPort, courtNumber);
            Trace.Indent();

            this.Id = id;
            this.CourtNumber = Math.Min(courtNumber, (byte)1);
            _recvrComm = new ReceiverSerialComm(commPort);
            _recvrComm.DataPacketReceived += RecvrComm_PacketReceivedHandler;
            Trace.Unindent();
        }

        #endregion

        #region Public Methods

        #if DEBUG
        /// <summary>
        /// Sends the bytes directly to receiver (debug mode only).
        /// </summary>
        /// <param name="data">The data.</param>
        public void SendDebugBytes(byte[] data)
        {
            Trace.TraceInformation("{0}.SendDebugBytes: {1}", CLASSNAME, BitConverter.ToString(data));
            Trace.Indent();
            if (_recvrComm == null) throw new InvalidOperationException("Receiver communications port is null");
            _recvrComm.SendDataPacket(data);
            Trace.Unindent();
        }
        #endif

        /// <summary>
        /// Gets an array of device registrations.
        /// </summary>
        /// <returns>RegistrationData array</returns>
        public ReadOnlyCollection<DeviceRegistrationEventData> GetDeviceRegistrations()
        {
            lock (_deviceRegistrations)
            {
                return _deviceRegistrations.Values.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Clears the device registrations from the receiver for the given game number.
        /// </summary>
        /// <param name="GameNumber">The game number.</param>
        public void ClearGameRegistration(byte gameNumber)
        {
            if (_recvrComm == null) throw new InvalidOperationException("Receiver communications port is null");
            Trace.TraceInformation("{0}.ClearGameRegistrations: Match={1}", CLASSNAME, gameNumber);
            Trace.Indent();
            try
            {
                _recvrComm.SendDataPacket(new byte[] { gameNumber, 0x60 }); // Returns 0x6F?
            }
            catch (IOException) { }

            var registrations = _deviceRegistrations.Where(kv => (gameNumber == 0 || kv.Value.GameNumber == gameNumber));
            foreach (var reg in registrations.ToArray())
            {
                Trace.TraceInformation("{0}.ClearGameRegistration: Removing {1} from registration collection", CLASSNAME, reg.Key);
                OnDeviceStatusUpdate(new DeviceStatusEventData() { Receiver = this, DeviceId = reg.Key, DeviceStatus = DeviceStatusEnum.NotResponding });
                _deviceRegistrations.Remove(reg.Key);
            }
            Trace.Unindent();
        }

        /// <summary>
        /// Clears the game registrations from the receiver.
        /// </summary>
        public void ClearGameRegistrations()
        {
            this.ClearGameRegistration(0);
            _deviceRegistrations.Clear();
        }

        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public void RegisterDevice(RegistrationSettings registration)
        {
            if (_recvrComm == null) throw new InvalidOperationException("Receiver communications port is null");

            if (registration.GameNumber < 1) throw new InvalidOperationException("Match Number must be between 1-255");
            if (registration.MinimumPressure < 10) throw new InvalidOperationException("Minimum Pressure Setting range is 10-255");

            Trace.TraceInformation("{0}.RegisterDevice: OpCode={1} Match={2} Court={3} RegSeq={4} MinPressure={5} Touch={6}",
                  CLASSNAME, registration.OpCode, registration.GameNumber, CourtNumber, _registrationSeq, registration.MinimumPressure, registration.TouchSensorMode);
            Trace.Indent();
            // Send registration command to receiver
            _recvrComm.SendDataPacket(new byte[6] 
            { 
                registration.GameNumber, 
                (byte)registration.OpCode,
                CourtNumber, 
                _registrationSeq, 
                registration.MinimumPressure,
                (byte)registration.TouchSensorMode 
            });

            // Increment the registration sequence number with wrap around
            _registrationSeq = (byte)(_registrationSeq + 1 % 0xFF);

            Trace.Unindent();
        }

        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public void PreRegisterDevice(PreRegistrationSettings registration)
        {
            if (_recvrComm == null) throw new InvalidOperationException("Receiver communications port is null");

            if (registration.GameNumber < 1) throw new InvalidOperationException("Match Number must be between 1-255");
            if (registration.CourtNumber < 1 || registration.CourtNumber > 8) throw new InvalidOperationException("Court Number must be between 1-8");

            Trace.TraceInformation("{0}.PreRegisterDevice: OpCode={1} Match={2} Court={3} RegSeq={4} Id={5}",
                  CLASSNAME, registration.OpCode, registration.GameNumber, registration.CourtNumber, registration.RegistrationSequence, 
                  registration.Id);
            Trace.Indent();

            // Send registration command to receiver
            _recvrComm.SendDataPacket(new byte[9] 
            {
                registration.GameNumber, 
                (byte)registration.OpCode,
                registration.CourtNumber,
                registration.RegistrationSequence, 
                0x00, // Not used
                0x00, // Not used
                registration.Id.ID1,
                registration.Id.ID2,
                registration.Id.ID3
            });

            Trace.Unindent();
        }


        /// <summary>
        /// Generates tone from the receiver speaker output.
        /// </summary>
        /// <param name="toneType">Type of the tone.</param>
        /// <returns></returns>
        public void GenerateTone(ToneTypeEnum toneType)
        {
            Trace.TraceInformation("{0}.GenerateTone: ToneType={1}", CLASSNAME, toneType);
            Trace.Indent();
            bool result = _recvrComm.SendDataPacket(new byte[3] { 0x01, (byte)OpCodeCmds.GenerateTone, (byte)toneType });
            Trace.Unindent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fired when valid data packet received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void RecvrComm_PacketReceivedHandler(object sender, DataPacketReceivedEventArgs e)
        {
            if (e.DataLength < 3) return;

            // Identify the packet and generate appropriate high level event
            OpCodes opCode = (OpCodes)e.DataPacket[1];

            Trace.TraceInformation("{0}.PacketReceivedHandler CommPort={1} OpCode={2} Length={3}", CLASSNAME, e.CommPort, opCode, e.DataLength);
            Trace.Indent();

            switch (opCode)
            {
                case OpCodes.PanelButtonPressed:
                    Publish_PanelButtonPress(e.DataPacket[2]);
                    break;

                #if EIDSS2
                case OpCodes.HongNotResponding:
                case OpCodes.ChungNotResponding:
                    Process_HoguNotResponding(opCode);
                    break;

                case OpCodes.ChungDataV1:
                case OpCodes.HongDataV1:
                    Publish_EventData( Process_HoguDataReceivedV1(e) );
                    break;
                #endif

                case OpCodes.ChungDataV2:
                case OpCodes.HongDataV2:
                    Publish_EventData ( Process_HoguDataReceivedV2(e) );
                    break;

                case OpCodes.TargetDataV2:
                    Publish_EventData ( Process_TargetDataReceivedV2(e) );
                    break;

                case OpCodes.HongRegistered:
                case OpCodes.ChungRegistered:
                case OpCodes.TargetRegistered:
                case OpCodes.HongPreRegistered:
                case OpCodes.ChungPreRegistered:
                    Publish_DeviceRegistration( Process_DeviceRegistration(e) );
                    break;

                default:
                    Trace.TraceWarning("{0}.RecvrComm_PacketReceivedHandler: Unknown packet type! Length={1} OpCode=0x{2:x2} Data={3}",
                        CLASSNAME, e.DataLength, (byte)opCode, BitConverter.ToString(e.DataPacket));
                    break;
            }
            Trace.Unindent();
        }

        /// <summary>
        /// Processes the device registration.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        private DeviceRegistrationEventData Process_DeviceRegistration(DataPacketReceivedEventArgs e)
        {
            // Parse data packet into registration data structure
            DeviceRegistrationEventData registrationData = new DeviceRegistrationEventData()
            {
                Receiver = this,
                GameNumber = e.DataPacket[(byte)RegistrationDataFields.GameNumber],
                OpCode = (OpCodes)e.DataPacket[(byte)RegistrationDataFields.OpCode],
                CourtNumber = e.DataPacket[(byte)RegistrationDataFields.CourtNumber],
                RegistrationSequence = e.DataPacket[(byte)RegistrationDataFields.RegSequence],
                MinimumPressure = e.DataPacket[(byte)RegistrationDataFields.MinimumPressure],
                TouchSensorMode = (TouchSensorStatusEnum)e.DataPacket[(byte)RegistrationDataFields.TouchSensorMode],
                Id = new DeviceId(
                    e.DataPacket[(byte)RegistrationDataFields.Id1],
                    e.DataPacket[(byte)RegistrationDataFields.Id2],
                    e.DataPacket[(byte)RegistrationDataFields.Id3]
                )
            };
            return registrationData;
        }

        /// <summary>
        /// Publishes the device registration.
        /// </summary>
        /// <param name="registrationData">The registration data.</param>
        private void Publish_DeviceRegistration(DeviceRegistrationEventData registrationData)
        {
            Trace.TraceInformation("{0}.Process_DeviceRegistration: OpCode={1} Match={2} Court={3} RegSeq={4} MinPressure={5} Touch={6} Id={7}",
                CLASSNAME, registrationData.OpCode, registrationData.GameNumber, registrationData.CourtNumber,
                registrationData.RegistrationSequence, registrationData.MinimumPressure, registrationData.TouchSensorMode,
                registrationData.Id);

            // Record registration sequence information for later verification
            lock (_deviceRegistrations)
            {
                if (_deviceRegistrations.ContainsKey(registrationData.Id))
                    _deviceRegistrations[registrationData.Id] = registrationData;
                else
                    _deviceRegistrations.Add(registrationData.Id, registrationData);
            }

            // Publish device registration event
            ThreadPool.QueueUserWorkItem(delegate
            {
                OnDeviceRegistered(registrationData);
            });

            #if EIDSS2
            // Keep track of Chung and Hong device Id as V1 (EIDSS 2.0) receiver does not provide them 
            // on data events unlike EIDSS 3.0 which does.
            switch (registrationData.OpCode)
            {
                case OpCodes.ChungRegistered:
                case OpCodes.ChungPreRegistered:
                    if (_HongDeviceId.Equals(registrationData.Id)) _HongDeviceId = new DeviceId();
                    _ChungDeviceId = registrationData.Id;
                    break;

                case OpCodes.HongRegistered:
                case OpCodes.HongPreRegistered:
                    if (_ChungDeviceId.Equals(registrationData.Id)) _ChungDeviceId = new DeviceId();
                    _HongDeviceId = registrationData.Id;
                    break;
            }
            #endif
        }

        /// <summary>
        /// Publish the event data.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        private void Publish_EventData(DeviceEventData deviceData)
        {
            // Check for duplicate packet
            if (deviceData.SequenceNumber == _lastSeqNumber)
                return;
            else
                _lastSeqNumber = deviceData.SequenceNumber;

            // Validate known device Id
            if ( !deviceData.DeviceId.IsValid() ||  !_deviceRegistrations.ContainsKey(deviceData.DeviceId) )
            {
                Trace.TraceWarning("{0}.Process_EventData: Unknown device id. ID={1}", CLASSNAME, deviceData.DeviceId);
                return;
            }

            // Validate registration sequence
            if (_deviceRegistrations[deviceData.DeviceId].RegistrationSequence != deviceData.RegSequence)
            {
                Trace.TraceWarning("{0}.Process_EventData: Incorrect registration sequence. ID={1} Expected={2} Actual={3}", CLASSNAME, deviceData.DeviceId, _deviceRegistrations[deviceData.DeviceId], deviceData.RegSequence);
                return;
            }

            // Trace/Debug Information
            if (Trace.Listeners.Count > 0) // Optimization for no listeners
                Trace.TraceInformation("{0}.Process_EventData: OpCode={1} Match={2} RegSeq={3} VestHit={4} HeadHit={5} Touch={6} Status={7} Id={8} TgtNum={9} TgtTot={10} Panel={11} Seq={12}", CLASSNAME,
                    deviceData.OpCode, deviceData.GameNumber, deviceData.RegSequence, deviceData.VestHitValue, 
                    deviceData.HeadHitValue, deviceData.TouchStatus, deviceData.DeviceStatus, deviceData.DeviceId, 
                    deviceData.TargetNumber, deviceData.TargetTotal, deviceData.WetBagPanel, deviceData.SequenceNumber);

            // Send device status update event
            ThreadPool.QueueUserWorkItem(delegate
            {
                OnDeviceStatusUpdate(new DeviceStatusEventData() { Receiver = this, DeviceId = deviceData.DeviceId, DeviceStatus = deviceData.DeviceStatus });
            });

            // If the pressure of head or vest is greater than zero send a strike impact event
            if (deviceData.HeadHitValue > 0 || deviceData.VestHitValue > 0)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    OnStrikeDetected(deviceData);
                });
            }
        }

        /// <summary>
        /// Processes the strike received for version 1 receiver (EIDSS 2.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        #if EIDSS2
        private DeviceEventData Process_HoguDataReceivedV1(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            _recvrComm.SendDataPacket(new byte[3] {
                e.DataPacket[(byte)HoguDataFieldsV1.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(byte)HoguDataFieldsV1.SequenceNumber] });

            // Parse data packet
            DeviceEventData deviceData = new DeviceEventData()
            {
                Receiver = this,
                GameNumber = e.DataPacket[(byte)HoguDataFieldsV1.GameNumber],
                OpCode = (OpCodes)e.DataPacket[(byte)HoguDataFieldsV1.OpCode],
                RegSequence = e.DataPacket[(byte)HoguDataFieldsV1.RegSequence],
                VestHitValue = e.DataPacket[(byte)HoguDataFieldsV1.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)HoguDataFieldsV1.HeadHitValue],
                TouchStatus = (TouchSensorStatusEnum)e.DataPacket[(byte)HoguDataFieldsV1.TouchStatus],
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)HoguDataFieldsV1.HoguStatus],
                SequenceNumber = e.DataPacket[(byte)HoguDataFieldsV1.SequenceNumber]
            };

            // Insert device Id information we stored during registration as V1 data packet does not include it
            switch (deviceData.OpCode)
            {
                case OpCodes.ChungDataV1: deviceData.DeviceId = _ChungDeviceId; break;
                case OpCodes.HongDataV1: deviceData.DeviceId = _HongDeviceId; break;
            }

            return deviceData;
        }

        /// <summary>
        /// Processes the device not responding (V1 receiver only).
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        private void Process_HoguNotResponding(OpCodes opCode)
        {
            DeviceId id = new DeviceId();
            switch (opCode)
            {
                case OpCodes.ChungNotResponding:
                    id = _ChungDeviceId;
                    break;

                case OpCodes.HongNotResponding:
                    id = _HongDeviceId;
                    break;
            }

            Trace.TraceInformation("{0}. Process_HoguNotResponding: {1} Id={2}", CLASSNAME, opCode, id);

            if (id.IsValid())
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    OnDeviceStatusUpdate(new DeviceStatusEventData() { Receiver = this, DeviceId = id, DeviceStatus = DeviceStatusEnum.NotResponding });
                });
            }
        }
        #endif

        /// <summary>
        /// Processes the strike received for V2 receiver (EIDSS 3.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        private DeviceEventData Process_HoguDataReceivedV2(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            _recvrComm.SendDataPacket(new byte[3] {
                e.DataPacket[(byte)HoguDataFieldsV2.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(byte)HoguDataFieldsV2.SequenceNumber] });

            // Parse data packet
            DeviceEventData deviceData = new DeviceEventData()
            {
                Receiver = this,
                GameNumber = e.DataPacket[(byte)HoguDataFieldsV2.GameNumber],
                OpCode = (OpCodes)e.DataPacket[(byte)HoguDataFieldsV2.OpCode],
                RegSequence = e.DataPacket[(byte)HoguDataFieldsV2.RegSequence],
                VestHitValue = e.DataPacket[(byte)HoguDataFieldsV2.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)HoguDataFieldsV2.HeadHitValue],
                TouchStatus = (TouchSensorStatusEnum)e.DataPacket[(byte)HoguDataFieldsV2.TouchStatus],
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)HoguDataFieldsV2.HoguStatus],
                DeviceId = new DeviceId(
                    e.DataPacket[(byte)HoguDataFieldsV2.ID1],
                    e.DataPacket[(byte)HoguDataFieldsV2.ID2],
                    e.DataPacket[(byte)HoguDataFieldsV2.ID3]
                ),
                SequenceNumber = e.DataPacket[(byte)HoguDataFieldsV2.SequenceNumber]
            };

            return deviceData;
        }

        /// <summary>
        /// Processes the strike received for V2 receiver (EIDSS 3.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        private DeviceEventData Process_TargetDataReceivedV2(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            _recvrComm.SendDataPacket(new byte[3] {
                e.DataPacket[(int)TargetDataFieldsV2.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(int)TargetDataFieldsV2.SequenceNumber] });

            // Parse data packet
            DeviceEventData deviceData = new DeviceEventData()
            {
                Receiver = this,
                GameNumber = e.DataPacket[(byte)TargetDataFieldsV2.GameNumber],
                OpCode = (OpCodes)e.DataPacket[(byte)TargetDataFieldsV2.OpCode],
                RegSequence = e.DataPacket[(byte)TargetDataFieldsV2.RegSequence],
                VestHitValue = e.DataPacket[(byte)TargetDataFieldsV2.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)TargetDataFieldsV2.HeadHitValue],
                // Remove "touch sensor" bit from wet bag panel byte by AND with 0x77 (0111 0111)
                WetBagPanel = (WetBagPanelEnum)(e.DataPacket[(byte)TargetDataFieldsV2.PanelValue] & 0x77),
                // Determine "touch sensor" bit from wet bag panel byte by checking 0x08 or 0x80 bits
                TouchStatus = (((e.DataPacket[(byte)TargetDataFieldsV2.PanelValue] & 0x08) == 0x08) || 
                               ((e.DataPacket[(byte)TargetDataFieldsV2.PanelValue] & 0x80) == 0x80)) ? 
                               TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)TargetDataFieldsV2.HoguStatus],
                DeviceId = new DeviceId(
                    e.DataPacket[(byte)TargetDataFieldsV2.ID1],
                    e.DataPacket[(byte)TargetDataFieldsV2.ID2],
                    e.DataPacket[(byte)TargetDataFieldsV2.ID3]
                ),
                SequenceNumber = e.DataPacket[(byte)TargetDataFieldsV2.SequenceNumber],
                TargetNumber = e.DataPacket[(byte)TargetDataFieldsV2.TargetNumber],
                TargetTotal = e.DataPacket[(byte)TargetDataFieldsV2.TargetTotal]
            };

            return deviceData;
        }

        /// <summary>
        /// Processes the panel button press.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        private void Publish_PanelButtonPress(byte ButtonCode)
        {
            PanelButtons panelButton = (PanelButtons)ButtonCode;
            Trace.TraceInformation("{0}.Process_PanelButtonPress: Keypad Button={1}", CLASSNAME, panelButton);
            ThreadPool.QueueUserWorkItem(delegate
            {
                OnPanelButtonPressed(new PanelButtonEventData() { 
                    Receiver = this, Button = panelButton } );
            });
        }

        #endregion

        #region Event Raising Methods
        /// <summary>
        /// Raises the <see cref="E:HitReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.HitReceivedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStrikeDetected(DeviceEventData e)
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
        protected virtual void OnDeviceStatusUpdate(DeviceStatusEventData e)
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
        protected virtual void OnDeviceRegistered(DeviceRegistrationEventData e)
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
        protected virtual void OnPanelButtonPressed(PanelButtonEventData e)
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
                if (_recvrComm != null)
                {
                    _recvrComm.DataPacketReceived -= RecvrComm_PacketReceivedHandler;
                    _recvrComm.Dispose();
                    _recvrComm = null;
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

}
