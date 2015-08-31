// <copyright file="ReceiverSerialComm.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;

    /// <summary>
    /// Methods to manage the low level communications to and from the receiver
    /// </summary>
    public class ReceiverSerialComm : IDisposable
    {
        #region Private Constants

        /// <summary>
        /// Class Name for Trace Logging
        /// </summary>
        private const string CLASSNAME = "ReceiverSerialComm";

        /// <summary>
        /// Baud Rate for Serial Port
        /// </summary>
        private const int BAUDRATE = 9600;

        /// <summary>
        /// Data Bits for Serial Port
        /// </summary>
        private const int DATABITS = 8;

        /// <summary>
        /// Parity Setting for Serial Port
        /// </summary>
        private const Parity PARITY = Parity.None;

        /// <summary>
        /// Stop Bit Setting for Serial Port
        /// </summary>
        private const StopBits STOPBITS = StopBits.One;

        /// <summary>
        /// Handshake Setting for Serial Port
        /// </summary>
        private const Handshake HANDSHAKE = Handshake.None;

        /// <summary>
        /// Timeout in milleseconds for writes to serial port
        /// </summary>
        private const int WRITETIMEOUT = 500;

        /// <summary>
        /// Maximum buffer size for serial port data
        /// </summary>
        private const int BUFFERSIZE = 256;

        #endregion

        #region Private Fields
        
        /// <summary>
        /// Serial Port Access Lock
        /// </summary>
        private readonly object portLock = new object();

        /// <summary>
        /// The Serial Port for Communications
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// Current State of the Receiver
        /// </summary>
        private ReceiverState receiverState;

        /// <summary>
        /// Serial data buffer
        /// </summary>
        private byte[] serialBuffer;

        /// <summary>
        /// Current Position in the Serial data buffer
        /// </summary>
        private int bufferPosition;

        /// <summary>
        /// Current length of data
        /// </summary>
        private int dataLength;

        /// <summary>
        /// Current this.checkSum of data
        /// </summary>
        private byte checkSum;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverSerialComm"/> class.
        /// </summary>
        /// <param name="commPort">The comm port.</param>
        public ReceiverSerialComm(string commPort)
        {
            Trace.TraceInformation("{0}.Constructor: CommPort={1}", CLASSNAME, commPort);
            Trace.Indent();
            this.OpenSerialPort(commPort);
            Trace.Unindent();
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Receiver"/> is reclaimed by garbage collection.
        /// </summary>
        ~ReceiverSerialComm()
        {
            throw new InvalidOperationException("ReceiverSerialComm Dispose method not called.");
        }
        #endif

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when [data packet received].
        /// </summary>
        public event EventHandler<DataPacketReceivedEventArgs> DataPacketReceived = delegate { };

        #endregion

        #region Private Enumerations
        /// <summary>
        /// Receiver State Machine
        /// </summary>
        private enum ReceiverState
        {
            /// <summary>
            /// No Receiver Device Found
            /// </summary>
            NoDeviceFound,

            /// <summary>
            /// Waiting for data
            /// </summary>
            Waiting,

            /// <summary>
            /// Expecting packet length byte
            /// </summary>
            PacketLength,

            /// <summary>
            /// Expecting packet data bytes
            /// </summary>
            PacketData,

            /// <summary>
            /// Expecting packet checksum byte
            /// </summary>
            checkSum,

            /// <summary>
            /// Received full packet
            /// </summary>
            Received,

            /// <summary>
            /// Sending packet
            /// </summary>
            Sending
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
        /// Sends the data packet to the receiver with wrapper
        /// </summary>
        /// <param name="data">The data packet.</param>
        /// <returns>Success if true</returns>
        public bool SendDataPacket(byte[] data)
        {
            lock (this.portLock)
            {
                if (this.serialPort != null && this.serialPort.IsOpen)
                {
                    try
                    {
                        byte checkSum = 0;
                        byte length = (byte)data.Length;
                        byte[] buffer = new byte[length + 4];
                        buffer[0] = 0x02;
                        buffer[1] = (byte)(length + 1);
                        Array.Copy(data, 0, buffer, 2, length);
                        for (int i = 1; i < (length + 2); i++)
                        {
                            checkSum ^= buffer[i];
                        }

                        buffer[length + 2] = checkSum;
                        buffer[length + 3] = 0x03;
                        this.serialPort.Write(buffer, 0, buffer.Length);
                        this.serialPort.BaseStream.Flush();
                        Debug.WriteLine(string.Format("{0}.SendDataPacket: CommPort={1} Data={2}", CLASSNAME, this.serialPort.PortName, BitConverter.ToString(buffer, 0, buffer.Length)));
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

        #region Protected Event Raising Methods

        /// <summary>
        /// Raises the <see cref="E:DataPacketReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Hardware.DataPacketReceivedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDataPacketReceived(DataPacketReceivedEventArgs e)
        {
            EventHandler<DataPacketReceivedEventArgs> handler = this.DataPacketReceived;
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0}.OnDataPacketReceived: {1}", CLASSNAME, ex.GetBaseException());
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
                this.CloseSerialPort();
            }

            Trace.Unindent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Opens the serial port.
        /// </summary>
        /// <param name="commPort">The comm port.</param>
        private void OpenSerialPort(string commPort)
        {
            lock (this.portLock)
            {
                // Open the serial port connection
                this.serialPort = new SerialPort()
                {
                    PortName = commPort,
                    BaudRate = BAUDRATE,
                    DataBits = DATABITS,
                    StopBits = STOPBITS,
                    Parity = PARITY,
                    WriteTimeout = WRITETIMEOUT,
                    Handshake = HANDSHAKE
                };

                Trace.TraceInformation("{0}.OpenSerialPort: CommPort={1}", CLASSNAME, this.serialPort.PortName);
                try
                {
                    this.serialPort.Open();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("{0}.OpenSerialPort: CommPort={1} Exception={2}", CLASSNAME, this.serialPort.PortName, ex.GetBaseException());
                    throw;
                }

                this.serialBuffer = new byte[BUFFERSIZE];
                this.bufferPosition = 0;
                this.receiverState = ReceiverState.Waiting;
                this.serialPort.DataReceived += this.SerialPort_DataReceived;
                this.serialPort.ErrorReceived += this.SerialPort_ErrorReceived;
                GC.SuppressFinalize(this.serialPort.BaseStream); // Work around, see http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/8a1825d2-c84b-4620-91e7-3934a4d47330
            } // end lock
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        private void CloseSerialPort()
        {
            lock (this.portLock)
            {
                Trace.TraceInformation("{0}.CloseSerialPort: CommPort={1}", CLASSNAME, this.serialPort.PortName);
                if (this.serialPort != null)
                {
                    this.serialPort.DataReceived -= this.SerialPort_DataReceived;
                    this.serialPort.ErrorReceived -= this.SerialPort_ErrorReceived;
                    if (this.serialPort.IsOpen)
                    {
                        try
                        {
                            this.serialPort.Close();
                        }
                        catch (IOException)
                        {
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("{0}.CloseSerialPort: CommPort={1} Exception={2}", CLASSNAME, this.serialPort.PortName, ex.GetBaseException());
                            throw;
                        }
                    }

                    this.serialPort.Dispose();
                    this.serialPort = null;

                    this.serialBuffer = null;
                    this.bufferPosition = 0;
                    this.receiverState = ReceiverState.NoDeviceFound;
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
            this.receiverState = ReceiverState.Waiting;
        }

        /// <summary>
        /// Handles the DataReceived event of the serialPort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.IO.Ports.SerialDataReceivedEventArgs"/> instance containing the event data.</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Data format: STX COUNT DATA ... this.checkSum ETX
            DataPacketReceivedEventArgs eventData = null;

            lock (this.portLock)
            {
                try
                {
                    while (this.serialPort.BytesToRead > 0)
                    {
                        byte data = (byte)this.serialPort.ReadByte();

                        switch (this.receiverState)
                        {
                            case ReceiverState.Waiting:
                                if (data == 0x02)
                                {
                                    this.receiverState = ReceiverState.PacketLength;
                                }

                                break;

                            case ReceiverState.PacketLength:
                                this.checkSum = data;
                                this.dataLength = data - 1;
                                this.bufferPosition = 0;
                                this.serialBuffer.Initialize();
                                this.receiverState = ReceiverState.PacketData;
                                break;

                            case ReceiverState.PacketData:
                                if (this.bufferPosition < this.dataLength)
                                {
                                    this.serialBuffer[this.bufferPosition++] = data;
                                    this.checkSum ^= data;
                                }

                                if (this.bufferPosition == this.dataLength)
                                {
                                    this.receiverState = ReceiverState.checkSum;
                                }

                                break;

                            case ReceiverState.checkSum:
                                if (this.checkSum != data)
                                {
                                    Trace.TraceWarning("{0}.SerialPort_DataReceived: this.checkSum mismatch. Expected={1} Actual={2}", CLASSNAME, this.checkSum, data);
                                    this.receiverState = ReceiverState.Waiting;
                                }
                                else
                                {
                                    this.receiverState = ReceiverState.Received;
                                }

                                break;

                            case ReceiverState.Received:
                                if (data == 0x03)
                                {
                                    // We got a message!
                                    eventData = new DataPacketReceivedEventArgs()
                                    {
                                        CommPort = this.serialPort.PortName,
                                        DataLength = this.dataLength,
                                        DataPacket = new byte[this.bufferPosition]
                                    };
                                    Array.Copy(this.serialBuffer, 0, eventData.DataPacket, 0, this.bufferPosition);
                                    this.receiverState = ReceiverState.Waiting;
                                }

                                break;

                            default:
                                Trace.TraceError("{0}.SerialPort_DataReceived: Unknown receive state, resetting state");
                                this.receiverState = ReceiverState.Waiting;
                                break;
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Trace.TraceError("{0}.SerialPort_DataReceived: CommPort={1} Exception={2}", CLASSNAME, this.serialPort.PortName, ex.GetBaseException());
                }
            } // end lock

            // Fire event if we have data (we are outside our lock here)
            if (eventData != null)
            {
                this.OnDataPacketReceived(eventData);
            }
        }

        #endregion
    }
}
