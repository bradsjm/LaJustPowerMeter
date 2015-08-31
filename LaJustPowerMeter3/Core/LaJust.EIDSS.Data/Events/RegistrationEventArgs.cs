// <copyright file="RegistrationEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Device Registration Data sent back by the Receiver when a device is registered
    /// </summary>
    public class DeviceRegistrationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver id.
        /// </summary>
        /// <value>The receiver id.</value>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Gets or sets the operation code.
        /// </summary>
        /// <value>The operation code.</value>
        public OpCodes OperationCode { get; set; }

        /// <summary>
        /// Gets or sets the match number.
        /// </summary>
        /// <value>The match number.</value>
        public byte GameNumber { get; set; }

        /// <summary>
        /// Gets or sets the court number.
        /// </summary>
        /// <value>The court number.</value>
        public byte CourtNumber { get; set; }

        /// <summary>
        /// Gets or sets the minimum pressure required for the device to report back a hit.
        /// </summary>
        /// <value>The minimum pressure.</value>
        public byte MinimumPressure { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether to require sock sensor. If required
        /// the device will not report back a hit unless the sock sensor is triggered.
        /// </summary>
        /// <value><c>true</c> if [require sock sensor]; otherwise, <c>false</c>.</value>
        public TouchSensorStatusEnum TouchSensorMode { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The device id.</value>
        public DeviceId Id { get; set; }

        /// <summary>
        /// Gets or sets the registration sequence.
        /// </summary>
        /// <value>The registration sequence.</value>
        public byte RegistrationSequence { get; set; }
    }
}
