// <copyright file="Receiver.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

// Enable this for legacy EIDSS 2.0 support (original receiver)
#define EIDSS2

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using LaJust.EIDSS.Communications.Entities;

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
        private const byte MAXCOURTS = 8;

        #endregion

        #region Private Fields

        /// <summary>
        /// Each registered device on this receiver is recorded in this dictionary.
        /// </summary>
        private Dictionary<DeviceId, DeviceRegistrationEventArgs> deviceRegistrations = new Dictionary<DeviceId, DeviceRegistrationEventArgs>();

        /// <summary>
        /// Court number to use. Each receiver should be assigned a unique court number (from 1 - 8)
        /// </summary>
        private byte courtNumber;

        /// <summary>
        /// The receiver serial communications object used to communicate to the
        /// device over USB.
        /// </summary>
        private ReceiverSerialComm receiverComm;

        /// <summary>
        /// Each packet from the receiver contains a sequence number. We track this to detect and ignore any
        /// duplicate packets.
        /// </summary>
        private byte lastSeqNumber = 0xFF;

        #if EIDSS2
        /// <summary>
        /// The original V1 receiver does not provide device ID information for each strike (V2 does). We keep
        /// track of the device Ids provided during registration so we can insert them in the strike packet.
        /// </summary>
        private DeviceId chungDeviceId = new DeviceId();

        /// <summary>
        /// The original V1 receiver does not provide device ID information for each strike (V2 does). We keep
        /// track of the device Ids provided during registration so we can insert them in the strike packet.
        /// </summary>
        private DeviceId hongDeviceId = new DeviceId();
        #endif

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver"/> class.
        /// </summary>
        /// <param name="id">The id of this receiver</param>
        /// <param name="commPort">The recvr serial port (e.g. COM3)</param>
        /// <param name="courtNumber">The court number assigned to this receiver (e.g. 1)</param>
        public Receiver(string id, string commPort, byte courtNumber)
        {
            CheckCallerAccess(Assembly.GetCallingAssembly());
            Trace.TraceInformation("{0}.Constructor: Id={1} CommPort={2} CourtNumber={3}", CLASSNAME, id, commPort, courtNumber);
            Trace.Indent();

            this.Id = id;
            this.CourtNumber = Math.Min(courtNumber, (byte)1);
            this.receiverComm = new ReceiverSerialComm(commPort);
            this.receiverComm.DataPacketReceived += this.RecvrComm_PacketReceivedHandler;
            Trace.Unindent();
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Receiver"/> is reclaimed by garbage collection.
        /// </summary>
        ~Receiver()
        {
            throw new InvalidOperationException("Receiver Dispose method not called.");
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

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the identification value for this object.
        /// </summary>
        /// <value>The device identifier.</value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the court number (1-8) assigned to this receiver.
        /// </summary>
        /// <value>The court number.</value>
        public byte CourtNumber
        {
            get { return this.courtNumber; }
            private set { this.courtNumber = (byte)(((value - 1) % MAXCOURTS) + 1); }  // Enforce court # must be from 1 through 8
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #if DEBUG
        /// <summary>
        /// Sends the bytes directly to receiver (debug mode only).
        /// </summary>
        /// <param name="dataPacket">The data packet.</param>
        public void SendDebugBytes(byte[] dataPacket)
        {
            Trace.TraceInformation("{0}.SendDebugBytes: {1}", CLASSNAME, BitConverter.ToString(dataPacket));
            Trace.Indent();
            if (this.receiverComm == null)
            {
                throw new InvalidOperationException("Receiver communications port is null");
            }

            this.receiverComm.SendDataPacket(dataPacket);
            Trace.Unindent();
        }
        #endif

        /// <summary>
        /// Gets an array of device registrations.
        /// </summary>
        /// <returns>RegistrationData array</returns>
        public ReadOnlyCollection<DeviceRegistrationEventArgs> GetDeviceRegistrations()
        {
            lock (this.deviceRegistrations)
            {
                return this.deviceRegistrations.Values.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Clears the device registrations from the receiver for the given game number.
        /// </summary>
        /// <param name="gameNumber">The game number.</param>
        public void ClearGameRegistration(byte gameNumber)
        {
            if (this.receiverComm == null)
            {
                throw new InvalidOperationException("Receiver communications port is null");
            }

            Trace.TraceInformation("{0}.ClearGameRegistrations: Match={1}", CLASSNAME, gameNumber);
            Trace.Indent();
            try
            {
                this.receiverComm.SendDataPacket(new byte[] { gameNumber, 0x60 }); // Returns 0x6F?
            }
            catch (IOException)
            {
            }

            var registrations = this.deviceRegistrations.Where(kv => (gameNumber == 0 || kv.Value.GameNumber == gameNumber));
            foreach (var reg in registrations.ToArray())
            {
                Trace.TraceInformation("{0}.ClearGameRegistration: Removing {1} from registration collection", CLASSNAME, reg.Key);
                this.OnDeviceStatusUpdate(new DeviceStatusEventArgs() { ReceiverId = this.Id, DeviceId = reg.Key, DeviceStatus = DeviceStatusEnum.NotResponding });
                this.deviceRegistrations.Remove(reg.Key);
            }

            Trace.Unindent();
        }

        /// <summary>
        /// Clears the game registrations from the receiver.
        /// </summary>
        public void ClearGameRegistrations()
        {
            this.ClearGameRegistration(0);
            this.deviceRegistrations.Clear();
        }

        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        public void RegisterDevice(RegistrationSettings registration)
        {
            if (this.receiverComm == null)
            {
                throw new InvalidOperationException("Receiver communications port is null");
            }

            if (registration.GameNumber < 1)
            {
                throw new InvalidOperationException("Game Number must be between 1-255");
            }

            if (registration.MinimumPressure < 30)
            {
                throw new InvalidOperationException("Minimum Pressure Setting range is 30-255");
            }

            Trace.TraceInformation(
                "{0}.RegisterDevice: OpCode={1} Match={2} Court={3} RegSeq={4} MinPressure={5} Touch={6}",
                CLASSNAME, 
                registration.OperationCode, 
                registration.GameNumber, 
                this.CourtNumber, 
                registration.RegistrationSequence, 
                registration.MinimumPressure, 
                registration.TouchSensorMode);
            Trace.Indent();

            // Send registration command to receiver
            this.receiverComm.SendDataPacket(new byte[6] 
            { 
                registration.GameNumber, 
                (byte)registration.OperationCode,
                this.CourtNumber, 
                registration.RegistrationSequence, 
                registration.MinimumPressure,
                (byte)registration.TouchSensorMode 
            });

            Trace.Unindent();
        }

        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        public void PreRegisterDevice(PreRegistrationSettings registration)
        {
            if (this.receiverComm == null)
            {
                throw new InvalidOperationException("Receiver communications port is null");
            }

            if (registration.GameNumber < 1)
            {
                throw new InvalidOperationException("Match Number must be between 1-255");
            }

            if (registration.CourtNumber < 1 || registration.CourtNumber > MAXCOURTS)
            {
                throw new InvalidOperationException("Court Number must be between 1-8");
            }

            Trace.TraceInformation(
                "{0}.PreRegisterDevice: OpCode={1} Match={2} Court={3} RegSeq={4} Id={5}",
                CLASSNAME, 
                registration.OperationCode, 
                registration.GameNumber, 
                registration.CourtNumber, 
                registration.RegistrationSequence, 
                registration.Id);
            Trace.Indent();

            // Send registration command to receiver
            this.receiverComm.SendDataPacket(new byte[9] 
            {
                registration.GameNumber, 
                (byte)registration.OperationCode,
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
        public void GenerateTone(ToneTypeEnum toneType)
        {
            Trace.TraceInformation("{0}.GenerateTone: ToneType={1}", CLASSNAME, toneType);
            Trace.Indent();
            bool result = this.receiverComm.SendDataPacket(new byte[3] { 0x01, (byte)OpCodeCmds.GenerateTone, (byte)toneType });
            Trace.Unindent();
        }

        #endregion

        #region Protected Event Raising Methods
        /// <summary>
        /// Raises the <see cref="E:HitReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Entities.DeviceDataEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStrikeDetected(DeviceDataEventArgs e)
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
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Entities.DeviceStatusEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDeviceStatusUpdate(DeviceStatusEventArgs e)
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
        /// <param name="e">The Device Registration Event Data.</param>
        protected virtual void OnDeviceRegistered(DeviceRegistrationEventArgs e)
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
        /// <param name="e">The Panel Button Event Data.</param>
        protected virtual void OnPanelButtonPressed(PanelButtonEventData e)
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
                if (this.receiverComm != null)
                {
                    this.receiverComm.DataPacketReceived -= this.RecvrComm_PacketReceivedHandler;
                    this.receiverComm.Dispose();
                    this.receiverComm = null;
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
        /// Fired when valid data packet received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The DataPacketReceivedEventArgs.</param>
        private void RecvrComm_PacketReceivedHandler(object sender, DataPacketReceivedEventArgs e)
        {
            if (e.DataLength < 3)
            {
                return;
            }

            // Identify the packet and generate appropriate high level event
            OpCodes operationCode = (OpCodes)e.DataPacket[1];

            Trace.TraceInformation(
                "{0}.PacketReceivedHandler CommPort={1} OpCode={2} Length={3}",
                CLASSNAME,
                e.CommPort,
                operationCode,
                e.DataLength);
            Trace.Indent();

            switch (operationCode)
            {
                case OpCodes.PanelButtonPressed:
                    this.Publish_PanelButtonPress(e.DataPacket[2]);
                    break;

#if EIDSS2
                case OpCodes.HongNotResponding:
                case OpCodes.ChungNotResponding:
                    this.Process_HoguNotResponding(operationCode);
                    break;

                case OpCodes.ChungDataV1:
                case OpCodes.HongDataV1:
                    this.Publish_EventData(this.Process_HoguDataReceivedV1(e));
                    break;
#endif

                case OpCodes.ChungDataV2:
                case OpCodes.HongDataV2:
                    this.Publish_EventData(this.Process_HoguDataReceivedV2(e));
                    break;

                case OpCodes.TargetDataV2:
                    this.Publish_EventData(this.Process_TargetDataReceivedV2(e));
                    break;

                case OpCodes.HongRegistered:
                case OpCodes.ChungRegistered:
                case OpCodes.TargetRegistered:
                case OpCodes.HongPreRegistered:
                case OpCodes.ChungPreRegistered:
                    this.Publish_DeviceRegistration(this.Process_DeviceRegistration(e));
                    break;

                default:
                    Trace.TraceWarning(
                        "{0}.RecvrComm_PacketReceivedHandler: Unknown packet type! Length={1} OpCode=0x{2:x2} Data={3}",
                        CLASSNAME,
                        e.DataLength,
                        (byte)operationCode,
                        BitConverter.ToString(e.DataPacket));
                    break;
            }

            Trace.Unindent();
        }

        /// <summary>
        /// Processes the device registration.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns>Device Registration Event Data</returns>
        private DeviceRegistrationEventArgs Process_DeviceRegistration(DataPacketReceivedEventArgs e)
        {
            // Parse data packet into registration data structure
            DeviceRegistrationEventArgs registrationData = new DeviceRegistrationEventArgs()
            {
                ReceiverId = this.Id,
                GameNumber = e.DataPacket[(byte)RegistrationDataFields.GameNumber],
                OperationCode = (OpCodes)e.DataPacket[(byte)RegistrationDataFields.OpCode],
                CourtNumber = e.DataPacket[(byte)RegistrationDataFields.CourtNumber],
                RegistrationSequence = e.DataPacket[(byte)RegistrationDataFields.RegSequence],
                MinimumPressure = e.DataPacket[(byte)RegistrationDataFields.MinimumImpact],
                TouchSensorMode = (TouchSensorStatusEnum)e.DataPacket[(byte)RegistrationDataFields.TouchSensorMode],
                Id = new DeviceId(e.DataPacket[(byte)RegistrationDataFields.Id1], e.DataPacket[(byte)RegistrationDataFields.Id2], e.DataPacket[(byte)RegistrationDataFields.Id3])
            };
            return registrationData;
        }

        /// <summary>
        /// Publishes the device registration.
        /// </summary>
        /// <param name="registrationData">The registration data.</param>
        private void Publish_DeviceRegistration(DeviceRegistrationEventArgs registrationData)
        {
            Trace.TraceInformation(
                "{0}.Process_DeviceRegistration: OpCode={1} Match={2} Court={3} RegSeq={4} MinPressure={5} Touch={6} Id={7}",
                CLASSNAME,
                registrationData.OperationCode,
                registrationData.GameNumber,
                registrationData.CourtNumber,
                registrationData.RegistrationSequence,
                registrationData.MinimumPressure,
                registrationData.TouchSensorMode,
                registrationData.Id);

            // Record registration sequence information for later verification
            lock (this.deviceRegistrations)
            {
                if (this.deviceRegistrations.ContainsKey(registrationData.Id))
                {
                    this.deviceRegistrations[registrationData.Id] = registrationData;
                }
                else
                {
                    this.deviceRegistrations.Add(registrationData.Id, registrationData);
                }
            }

            // Publish device registration event
            ThreadPool.QueueUserWorkItem(delegate
            {
                this.OnDeviceRegistered(registrationData);
            });

#if EIDSS2
            // Keep track of Chung and Hong device Id as V1 (EIDSS 2.0) receiver does not provide them 
            // on data events unlike EIDSS 3.0 which does.
            switch (registrationData.OperationCode)
            {
                case OpCodes.ChungRegistered:
                case OpCodes.ChungPreRegistered:
                    if (this.hongDeviceId.Equals(registrationData.Id))
                    {
                        this.hongDeviceId = new DeviceId();
                    }

                    this.chungDeviceId = registrationData.Id;
                    break;

                case OpCodes.HongRegistered:
                case OpCodes.HongPreRegistered:
                    if (this.chungDeviceId.Equals(registrationData.Id))
                    {
                        this.chungDeviceId = new DeviceId();
                    }

                    this.hongDeviceId = registrationData.Id;
                    break;
            }
#endif
        }

        /// <summary>
        /// Publish the event data.
        /// </summary>
        /// <param name="deviceData">The device data.</param>
        private void Publish_EventData(DeviceDataEventArgs deviceData)
        {
            // Check for duplicate packet
            if (deviceData.SequenceNumber == this.lastSeqNumber)
            {
                return;
            }
            else
            {
                this.lastSeqNumber = deviceData.SequenceNumber;
            }

            // Validate known device Id
            if (!deviceData.DeviceId.IsValid() || !this.deviceRegistrations.ContainsKey(deviceData.DeviceId))
            {
                Trace.TraceWarning("{0}.Process_EventData: Unknown device id. ID={1}", CLASSNAME, deviceData.DeviceId);
                return;
            }

            // Validate registration sequence
            if (this.deviceRegistrations[deviceData.DeviceId].RegistrationSequence != deviceData.RegistrationSequence)
            {
                Trace.TraceWarning("{0}.Process_EventData: Incorrect registration sequence. ID={1} Expected={2} Actual={3}", CLASSNAME, deviceData.DeviceId, this.deviceRegistrations[deviceData.DeviceId], deviceData.RegistrationSequence);
                return;
            }

            // Trace/Debug Information (only if we have listeners)
            if (Trace.Listeners.Count > 0)
            {
                Trace.TraceInformation(
                    "{0}.Process_EventData: OpCode={1} Match={2} RegSeq={3} VestHit={4} HeadHit={5} Touch={6} Status={7} Id={8} TgtNum={9} TgtTot={10} Panel={11} Seq={12}",
                    CLASSNAME,
                    deviceData.OperationCode,
                    deviceData.GameNumber,
                    deviceData.RegistrationSequence,
                    deviceData.VestHitValue,
                    deviceData.HeadHitValue,
                    deviceData.TouchStatus,
                    deviceData.DeviceStatus,
                    deviceData.DeviceId,
                    deviceData.TargetNumber,
                    deviceData.TargetTotal,
                    deviceData.WetBagPanel,
                    deviceData.SequenceNumber);
            }

            // Send device status update event
            ThreadPool.QueueUserWorkItem(delegate
            {
                this.OnDeviceStatusUpdate(new DeviceStatusEventArgs() { ReceiverId = this.Id, DeviceId = deviceData.DeviceId, DeviceStatus = deviceData.DeviceStatus });
            });

            // If the pressure of head or vest is greater than zero send a strike impact event
            if (deviceData.HeadHitValue > 0 || deviceData.VestHitValue > 0)
            {
                // code to software patch sensor issue 12/19/09
                if (deviceData.WetBagPanel == WetBagPanelEnum.BottomMiddle)
                {
                    deviceData.VestHitValue = (byte)((double)deviceData.VestHitValue * 1.20);
                }

                ThreadPool.QueueUserWorkItem(delegate
                {
                    this.OnStrikeDetected(deviceData);
                });
            }
        }

        /// <summary>
        /// Processes the strike received for version 1 receiver (EIDSS 2.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns>Device Event Data</returns>
#if EIDSS2
        private DeviceDataEventArgs Process_HoguDataReceivedV1(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            this.receiverComm.SendDataPacket(new byte[3] 
            {
                e.DataPacket[(byte)HoguDataFieldsV1.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(byte)HoguDataFieldsV1.SequenceNumber] 
            });

            // Parse data packet
            DeviceDataEventArgs deviceData = new DeviceDataEventArgs()
            {
                ReceiverId = this.Id,
                GameNumber = e.DataPacket[(byte)HoguDataFieldsV1.GameNumber],
                OperationCode = (OpCodes)e.DataPacket[(byte)HoguDataFieldsV1.OpCode],
                RegistrationSequence = e.DataPacket[(byte)HoguDataFieldsV1.RegSequence],
                VestHitValue = e.DataPacket[(byte)HoguDataFieldsV1.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)HoguDataFieldsV1.HeadHitValue],
                TouchStatus = (TouchSensorStatusEnum)e.DataPacket[(byte)HoguDataFieldsV1.TouchStatus],
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)HoguDataFieldsV1.HoguStatus],
                SequenceNumber = e.DataPacket[(byte)HoguDataFieldsV1.SequenceNumber]
            };

            // Insert device Id information we stored during registration as V1 data packet does not include it
            switch (deviceData.OperationCode)
            {
                case OpCodes.ChungDataV1:
                    deviceData.DeviceId = this.chungDeviceId;
                    break;

                case OpCodes.HongDataV1:
                    deviceData.DeviceId = this.hongDeviceId;
                    break;
            }

            return deviceData;
        }

        /// <summary>
        /// Processes the device not responding (V1 receiver only).
        /// </summary>
        /// <param name="operationCode">The operation code.</param>
        private void Process_HoguNotResponding(OpCodes operationCode)
        {
            DeviceId id = new DeviceId();
            switch (operationCode)
            {
                case OpCodes.ChungNotResponding:
                    id = this.chungDeviceId;
                    break;

                case OpCodes.HongNotResponding:
                    id = this.hongDeviceId;
                    break;
            }

            Trace.TraceInformation("{0}. Process_HoguNotResponding: {1} Id={2}", CLASSNAME, operationCode, id);

            if (id.IsValid())
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    this.OnDeviceStatusUpdate(new DeviceStatusEventArgs() { ReceiverId = this.Id, DeviceId = id, DeviceStatus = DeviceStatusEnum.NotResponding });
                });
            }
        }
#endif

        /// <summary>
        /// Processes the strike received for V2 receiver (EIDSS 3.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns>Device Event Data</returns>
        private DeviceDataEventArgs Process_HoguDataReceivedV2(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            this.receiverComm.SendDataPacket(new byte[3] 
            {
                e.DataPacket[(byte)HoguDataFieldsV2.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(byte)HoguDataFieldsV2.SequenceNumber] 
            });

            // Parse data packet
            DeviceDataEventArgs deviceData = new DeviceDataEventArgs()
            {
                ReceiverId = this.Id,
                GameNumber = e.DataPacket[(byte)HoguDataFieldsV2.GameNumber],
                OperationCode = (OpCodes)e.DataPacket[(byte)HoguDataFieldsV2.OpCode],
                RegistrationSequence = e.DataPacket[(byte)HoguDataFieldsV2.RegSequence],
                VestHitValue = e.DataPacket[(byte)HoguDataFieldsV2.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)HoguDataFieldsV2.HeadHitValue],
                TouchStatus = (TouchSensorStatusEnum)e.DataPacket[(byte)HoguDataFieldsV2.TouchStatus],
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)HoguDataFieldsV2.HoguStatus],
                DeviceId = new DeviceId(e.DataPacket[(byte)HoguDataFieldsV2.ID1], e.DataPacket[(byte)HoguDataFieldsV2.ID2], e.DataPacket[(byte)HoguDataFieldsV2.ID3]),
                SequenceNumber = e.DataPacket[(byte)HoguDataFieldsV2.SequenceNumber]
            };

            return deviceData;
        }

        /// <summary>
        /// Processes the strike received for V2 receiver (EIDSS 3.0)
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        /// <returns>Device Event Data</returns>
        private DeviceDataEventArgs Process_TargetDataReceivedV2(DataPacketReceivedEventArgs e)
        {
            // Generate acknowledgement packet and send to receiver
            this.receiverComm.SendDataPacket(new byte[3] 
            {
                e.DataPacket[(int)TargetDataFieldsV2.GameNumber], 
                (byte)OpCodeCmds.Acknowledgement, 
                e.DataPacket[(int)TargetDataFieldsV2.SequenceNumber] 
            });

            // Parse data packet
            DeviceDataEventArgs deviceData = new DeviceDataEventArgs()
            {
                ReceiverId = this.Id,
                GameNumber = e.DataPacket[(byte)TargetDataFieldsV2.GameNumber],
                OperationCode = (OpCodes)e.DataPacket[(byte)TargetDataFieldsV2.OpCode],
                RegistrationSequence = e.DataPacket[(byte)TargetDataFieldsV2.RegSequence],
                VestHitValue = e.DataPacket[(byte)TargetDataFieldsV2.VestHitValue],
                HeadHitValue = e.DataPacket[(byte)TargetDataFieldsV2.HeadHitValue],

                // Remove "touch sensor" bit from wet bag panel byte by AND with 0x77 (0111 0111)
                WetBagPanel = (WetBagPanelEnum)(e.DataPacket[(byte)TargetDataFieldsV2.TargetPanel] & 0x77),

                // Determine "touch sensor" bit from wet bag panel byte by checking 0x08 or 0x80 bits
                TouchStatus = (((e.DataPacket[(byte)TargetDataFieldsV2.TargetPanel] & 0x08) == 0x08) ||
                               ((e.DataPacket[(byte)TargetDataFieldsV2.TargetPanel] & 0x80) == 0x80)) ?
                               TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                DeviceStatus = (DeviceStatusEnum)e.DataPacket[(byte)TargetDataFieldsV2.TargetStatus],
                DeviceId = new DeviceId(e.DataPacket[(byte)TargetDataFieldsV2.ID1], e.DataPacket[(byte)TargetDataFieldsV2.ID2], e.DataPacket[(byte)TargetDataFieldsV2.ID3]),
                SequenceNumber = e.DataPacket[(byte)TargetDataFieldsV2.SequenceNumber],
                TargetNumber = e.DataPacket[(byte)TargetDataFieldsV2.TargetNumber],
                TargetTotal = e.DataPacket[(byte)TargetDataFieldsV2.TargetTotal]
            };

            return deviceData;
        }

        /// <summary>
        /// Processes the panel button press.
        /// </summary>
        /// <param name="buttonCode">The button code.</param>
        private void Publish_PanelButtonPress(byte buttonCode)
        {
            PanelButtons panelButton = (PanelButtons)buttonCode;
            Trace.TraceInformation("{0}.Process_PanelButtonPress: Keypad Button={1}", CLASSNAME, panelButton);
            ThreadPool.QueueUserWorkItem(delegate
            {
                this.OnPanelButtonPressed(new PanelButtonEventData()
                {
                    ReceiverId = this.Id,
                    Button = panelButton
                });
            });
        }

        #endregion
    }
}
