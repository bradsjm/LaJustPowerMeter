/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.EIDSS.Communications.Entities
{
    using System;
using LaJust.EIDSS.Communications.Hardware;

    /// <summary>
    /// Device Registration Data sent back by the Receiver when a device is registered
    /// </summary>
    public class DeviceRegistrationEventData : EventArgs
    {
        /// <summary>
        /// Gets or sets the receiver.
        /// </summary>
        /// <value>The receiver.</value>
        public IReceiver Receiver { get; set; }
        /// <summary>
        /// Gets or sets the hogu.
        /// </summary>
        /// <value>The hogu.</value>
        public OpCodes OpCode { get; set; }
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
        /// Gets or sets a value indicating whether to require sock sensor. If required
        /// the device will not report back a hit unless the sock sensor is triggered.
        /// </summary>
        /// <value><c>true</c> if [require sock sensor]; otherwise, <c>false</c>.</value>
        public TouchSensorStatusEnum TouchSensorMode { get; set; }
        /// <summary>
        /// Device Id
        /// </summary>
        /// <value>The ID code.</value>
        public DeviceId Id { get; set; }
        /// <summary>
        /// The registration sequence number
        /// </summary>
        public byte RegistrationSequence { get; set; }
    }
}
