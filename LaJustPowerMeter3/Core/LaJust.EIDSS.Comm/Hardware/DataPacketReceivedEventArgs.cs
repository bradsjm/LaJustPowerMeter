// <copyright file="DataPacketReceivedEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;

    /// <summary>
    /// Event Handler for data packet received from controller
    /// </summary>
    public class DataPacketReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the comm port.
        /// </summary>
        /// <value>The comm port.</value>
        public string CommPort { get; internal set; }

        /// <summary>
        /// Gets the length of the data.
        /// </summary>
        /// <value>The length of the data.</value>
        public int DataLength { get; internal set; }

        /// <summary>
        /// Gets the data packet.
        /// </summary>
        /// <value>The data packet.</value>
        public byte[] DataPacket { get; internal set; }
    }
}
