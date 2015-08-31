/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    public struct RegistrationSettings
    {
        /// <summary>
        /// Gets or sets the hogu.
        /// </summary>
        /// <value>The hogu.</value>
        public OpCodeCmds OpCode { get; set; }
        /// <summary>
        /// Gets or sets the match number.
        /// </summary>
        /// <value>The match number. Range 1 - 255</value>
        public byte GameNumber { get; set; }
        /// <summary>
        /// Gets or sets the minimum pressure required for the device to report back a hit. Range 10-255.
        /// </summary>
        /// <value>The minimum pressure.</value>
        public byte MinimumPressure { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to require sock sensor. If required
        /// the device will not report back a hit unless the sock sensor is triggered.
        /// </summary>
        /// <value><c>true</c> if [require sock sensor]; otherwise, <c>false</c>.</value>
        public TouchSensorStatusEnum TouchSensorMode { get; set; }
    }

    public struct PreRegistrationSettings
    {
        /// <summary>
        /// Gets or sets the op code.
        /// </summary>
        /// <value>The op code.</value>
        public OpCodeCmds OpCode { get; set; }
        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber { get; set; }
        /// <summary>
        /// Gets or sets the court number.
        /// </summary>
        /// <value>The court number.</value>
        public byte CourtNumber { get; set; }
        /// <summary>
        /// Gets or sets the registration sequence.
        /// </summary>
        /// <value>The registration sequence.</value>
        public byte RegistrationSequence { get; set; }
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public DeviceId Id { get; set; }
    }
}
