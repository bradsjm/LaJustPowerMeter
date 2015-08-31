// <copyright file="DeviceIdEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Event Arguments used for passing a device id
    /// </summary>
    public class DeviceIdEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the device id.
        /// </summary>
        /// <value>The device id.</value>
        public DeviceId DeviceId { get; set; }
    }
}
