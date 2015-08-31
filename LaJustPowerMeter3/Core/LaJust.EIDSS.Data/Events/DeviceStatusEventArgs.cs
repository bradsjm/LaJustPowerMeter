// <copyright file="DeviceStatusEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Device Status Event Data
    /// </summary>
    public class DeviceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver id.
        /// </summary>
        /// <value>The recevier id.</value>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>The device id.</value>
        public DeviceId DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device status.
        /// </summary>
        /// <value>The device status.</value>
        public DeviceStatusEnum DeviceStatus { get; set; }
    }
}
