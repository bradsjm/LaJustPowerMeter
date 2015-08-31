// <copyright file="VirtualComPortEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management;
    using System.Threading;

    /// <summary>
    /// Virtual Comm Port Event Insert/Removal Arguments
    /// </summary>
    public class VirtualComPortEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; internal set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the device ID.
        /// </summary>
        /// <value>The device ID.</value>
        public string DeviceID { get; internal set; }

        /// <summary>
        /// Gets the name of the port.
        /// </summary>
        /// <value>The name of the port.</value>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the PNP device id.
        /// </summary>
        /// <value>The PNP device id.</value>
        public string PnpDeviceId { get; internal set; }

        #endregion
    }
}
