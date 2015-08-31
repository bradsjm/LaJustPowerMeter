/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.IO.Ports;
    using System.Reflection;
    using System.Threading;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// Event Handler for data packet received from controller
    /// </summary>
    internal class DataPacketReceivedEventArgs : EventArgs
    {
        #region Public Properties
        public string CommPort { get; internal set; }
        public int DataLength { get; internal set; }
        public byte[] DataPacket { get; internal set; }
        #endregion
    }

    /// <summary>
    /// Methods to manage the low level communications to and from the receiver
    /// </summary>
    internal class ReceiverSerialComm : IDisposable
    {
        #region Private Constants

        private const string CLASSNAME = "ReceiverSerialComm";
        private const int BAUD_RATE = 9600;
        private const int DATA_BITS = 8;
        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HANDSHAKE = Handshake.None;
        private const int WRITE_TIMEOUT = 500;
        private const int BUFFER_SIZE = 256;

        #endregion

        #region Private Enumerations
        /// <summary>
        /// Receiver State Machine
        /// </summary>
        enum RecvrState
        {
            NoDeviceFound,
            Waiting,
            PacketLength,
            PacketData,
            Checksum,
            Received,
            Sending
        }
        #endregion

        #region Private Fields

        private SerialPort _serialPort;
        private RecvrState _state;
        private byte[] _buffer;
        private int _bufPos;
        private int _dataLength;
        private byte _checksum;
        private readonly object _lock = new object();

        #endregion

        #region Public Events

        public event EventHandler<DataPacketReceivedEventArgs> DataPacketReceived = delegate { };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RecvrComm"/> class.
        /// </summary>
        public ReceiverSerialComm(string commPort)
        {
            Trace.TraceInformation("{0}.Constructor: CommPort={1}", CLASSNAME, commPort);
            Trace.Indent();
            OpenSerialPort(commPort);
            Trace.Unindent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the data packet to the receiver with wrapper
        /// </summary>
        /// <param name="data">The data packet.</param>
        public bool SendDataPacket(byte[] data)
        {
            lock (_lock)
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    try
                    {
                        byte checksum = 0;
                        byte length = (byte)data.Length;
                        byte[] buffer = new byte[length + 4];
                        buffer[0] = 0x02;
                        buffer[1] = (byte)(length + 1);
                        Array.Copy(data, 0, buffer, 2, length);
                        for (int i = 1; i < (length + 2); i++) checksum ^= buffer[i];
                        buffer[length + 2] = checksum;
                        buffer[length + 3] = 0x03;
                        _serialPort.Write(buffer, 0, buffer.Length);
                        _serialPort.BaseStream.Flush();
                        Debug.WriteLine(string.Format("{0}.SendDataPacket: CommPort={1} Data={2}", CLASSNAME, _serialPort.PortName, BitConverter.ToString(buffer, 0, buffer.Length)));
                        return true;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Trace.TraceError("{0}.SendDataPacket: {1}", CLASSNAME, ex.GetBaseException());
                    }
                }
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opens the serial port.
        /// </summary>
        private void OpenSerialPort(string commPort)
        {
            lock (_lock)
            {
                // Open the serial port connection
                _serialPort = new SerialPort()
                {
                    PortName = commPort,
                    BaudRate = BAUD_RATE,
                    DataBits = DATA_BITS,
                    StopBits = STOP_BITS,
                    Parity = PARITY,
                    WriteTimeout = WRITE_TIMEOUT,
                    Handshake = HANDSHAKE
                };

                Trace.TraceInformation("{0}.OpenSerialPort: CommPort={1}", CLASSNAME, _serialPort.PortName);
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}.OpenSerialPort: CommPort={1} Exception={2}", CLASSNAME, _serialPort.PortName, ex.GetBaseException());
                    throw ex;
                }

                _buffer = new byte[BUFFER_SIZE];
                _bufPos = 0;
                _state = RecvrState.Waiting;
                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.ErrorReceived += SerialPort_ErrorReceived;
                GC.SuppressFinalize(_serialPort.BaseStream); // Work around, see http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/8a1825d2-c84b-4620-91e7-3934a4d47330
            } // end lock
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        private void CloseSerialPort()
        {
            lock (_lock)
            {
                Trace.TraceInformation("{0}.CloseSerialPort: CommPort={1}", CLASSNAME, _serialPort.PortName);
                // Attempt to close the serial port object
                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.ErrorReceived -= SerialPort_ErrorReceived;
                    if (_serialPort.IsOpen)
                    {
                        try
                        {
                            _serialPort.Close();
                        }
                        catch (IOException) { }
                        catch (Exception ex)
                        {
                            Trace.TraceError("{0}.CloseSerialPort: CommPort={1} Exception={2}", CLASSNAME, _serialPort.PortName, ex.GetBaseException());
                            throw ex;
                        }
                    }
                    _serialPort.Dispose();
                    _serialPort = null;

                    _buffer = null;
                    _bufPos = 0;
                    _state = RecvrState.NoDeviceFound;
                }
            } // end lock
        }

        /// <summary>
        /// Handles the ErrorReceived event of the serialPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.Ports.SerialErrorReceivedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            _state = RecvrState.Waiting;
        }

        /// <summary>
        /// Handles the DataReceived event of the serialPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.Ports.SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Data format: STX COUNT DATA ... CHECKSUM ETX
            DataPacketReceivedEventArgs eventData = null;

            lock (_lock)
            {
                try
                {
                    while (_serialPort.BytesToRead > 0)
                    {
                        byte data = (byte)_serialPort.ReadByte();

                        switch (_state)
                        {
                            case RecvrState.Waiting:
                                if (data == 0x02)
                                {
                                    _state = RecvrState.PacketLength;
                                }
                                break;

                            case RecvrState.PacketLength:
                                _checksum = data;
                                _dataLength = data - 1;
                                _bufPos = 0;
                                _buffer.Initialize();
                                _state = RecvrState.PacketData;
                                break;

                            case RecvrState.PacketData:
                                if (_bufPos < _dataLength)
                                {
                                    _buffer[_bufPos++] = data;
                                    _checksum ^= data;
                                }

                                if (_bufPos == _dataLength)
                                {
                                    _state = RecvrState.Checksum;
                                }
                                break;

                            case RecvrState.Checksum:
                                if (_checksum != data)
                                {
                                    Trace.TraceWarning("{0}.SerialPort_DataReceived: Checksum mismatch. Expected={1} Actual={2}", CLASSNAME, _checksum, data);
                                    _state = RecvrState.Waiting;
                                }
                                else
                                {
                                    _state = RecvrState.Received;
                                }
                                break;

                            case RecvrState.Received:
                                if (data == 0x03)
                                {
                                    // We got a message!
                                    eventData = new DataPacketReceivedEventArgs() 
                                    {
                                        CommPort = _serialPort.PortName,
                                        DataLength = _dataLength,
                                        DataPacket = new byte[_bufPos]
                                    };
                                    Array.Copy(_buffer, 0, eventData.DataPacket, 0, _bufPos);
                                    _state = RecvrState.Waiting;
                                }
                                break;

                            default:
                                Trace.TraceError("{0}.SerialPort_DataReceived: Unknown receive state, resetting state");
                                _state = RecvrState.Waiting;
                                break;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Trace.TraceError("{0}.SerialPort_DataReceived: CommPort={1} Exception={2}", CLASSNAME, _serialPort.PortName, ex.GetBaseException());
                }
            } // end lock

            // Fire event if we have data (we are outside our lock here)
            if (eventData != null) OnDataPacketReceived(eventData);
        }

        #endregion

        #region Event Raising Methods

        /// <summary>
        /// Raises the <see cref="E:DataPacketReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDataPacketReceived(DataPacketReceivedEventArgs e)
        {
            EventHandler<DataPacketReceivedEventArgs> handler = DataPacketReceived;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDataPacketReceived: {1}", CLASSNAME, ex.GetBaseException());
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
                CloseSerialPort();
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
    }
}
