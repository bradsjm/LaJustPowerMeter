﻿// <copyright file="DeviceDataEventArgs.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Device Event Data Generated by Receiver
    /// </summary>
    public class DeviceDataEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver id.
        /// </summary>
        /// <value>The receiver guid.</value>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The match number.</value>
        public byte GameNumber { get; set; }

        /// <summary>
        /// Gets or sets the operation code.
        /// </summary>
        /// <value>The operation code.</value>
        public OpCodes OperationCode { get; set; }

        /// <summary>
        /// Gets or sets the registration sequence.
        /// </summary>
        /// <value>The registration sequence number.</value>
        public byte RegistrationSequence { get; set; }

        /// <summary>
        /// Gets or sets the vest hit value.
        /// </summary>
        /// <value>The vest hit impact sensor value..</value>
        public byte VestHitValue { get; set; }

        /// <summary>
        /// Gets or sets the touch status.
        /// </summary>
        /// <value>The contact sensor.</value>
        public TouchSensorStatusEnum TouchStatus { get; set; }

        /// <summary>
        /// Gets or sets the wet bag panel.
        /// </summary>
        /// <value>The wet bag panel.</value>
        public WetBagPanelEnum WetBagPanel { get; set; }

        /// <summary>
        /// Gets or sets the head hit value.
        /// </summary>
        /// <value>The vest hit impact sensor value.</value>
        public byte HeadHitValue { get; set; }

        /// <summary>
        /// Gets or sets the device status.
        /// </summary>
        /// <value>The device status.</value>
        public DeviceStatusEnum DeviceStatus { get; set; }

        /// <summary>
        /// Gets or sets the Device identification ID bytes.
        /// </summary>
        /// <value>The ID number.</value>
        public DeviceId DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the Sequence number.
        /// </summary>
        /// <value>The sequence number.</value>
        public byte SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the Target Number
        /// </summary>
        public byte TargetNumber { get; set; }

        /// <summary>
        /// Gets or sets the Target Totals
        /// </summary>
        public byte TargetTotal { get; set; }
    }
}
