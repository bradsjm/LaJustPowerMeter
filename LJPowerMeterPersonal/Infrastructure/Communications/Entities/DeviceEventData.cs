﻿/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.EIDSS.Communications.Entities
{
    using System;
    using LaJust.EIDSS.Communications.Hardware;

    public class DeviceStatusEventData : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <value>The recevier.</value>
        public IReceiver Receiver { get; set; }
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

    /// <summary>
    /// Device Event Data Generated by Receiver
    /// </summary>
    public class DeviceEventData : EventArgs
    {
        /// <summary>
        /// ID string of the receiver
        /// </summary>
        public IReceiver Receiver { get; set; }
        /// <summary>
        /// Game number.
        /// </summary>
        /// <value>The match number.</value>
        public byte GameNumber { get; set; }
        /// <summary>
        /// Sensor Device.
        /// </summary>
        /// <value>The device.</value>
        public OpCodes OpCode { get; set; }
        /// <summary>
        /// Time code.
        /// </summary>
        /// <value>The time code.</value>
        public byte RegSequence { get; set; }
        /// <summary>
        /// Pressure sensor.
        /// </summary>
        /// <value>The pressure sensor.</value>
        public byte VestHitValue { get; set; }
        /// <summary>
        /// Contact sensor mode.
        /// </summary>
        /// <value>The contact sensor.</value>
        public TouchSensorStatusEnum TouchStatus { get; set; }
        /// <summary>
        /// Wet Bag Panel
        /// </summary>
        public WetBagPanelEnum WetBagPanel { get; set; }
        /// <summary>
        /// Gets or sets the HeadHitValue.
        /// </summary>
        /// <value>The HeadHitValue.</value>
        public byte HeadHitValue { get; set; }
        /// <summary>
        /// Gets or sets the Hogu Status.
        /// </summary>
        /// <value>The hogu status.</value>
        public DeviceStatusEnum DeviceStatus { get; set; }
        /// <summary>
        /// Device identification ID bytes.
        /// </summary>
        /// <value>The ID number.</value>
        public DeviceId DeviceId { get; set; }
        /// <summary>
        /// Sequence number.
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
