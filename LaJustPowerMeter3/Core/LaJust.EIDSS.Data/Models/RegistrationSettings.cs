// <copyright file="RegistrationSettings.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Entities
{
    using System;

    /// <summary>
    /// Model structure for device pre-registration settings
    /// </summary>
    public struct PreRegistrationSettings
    {
        /// <summary>
        /// Gets or sets the op code.
        /// </summary>
        /// <value>The op code.</value>
        public OpCodeCmds OperationCode { get; set; }

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
        /// <value>The device id.</value>
        public DeviceId Id { get; set; }
    }

    /// <summary>
    /// Model for Device Registration Settings
    /// </summary>
    public class RegistrationSettings
    {
        /// <summary>
        /// The Registration Sequence number starting value is randomized based on clock ticks since boot.
        /// It is incremented for each registration on any receiver so we use a static member.
        /// </summary>
        private static byte registrationSeq = (byte)(Environment.TickCount % 0xFF);

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationSettings"/> class.
        /// </summary>
        public RegistrationSettings()
        {
            this.RegistrationSequence = registrationSeq;
            this.MinimumPressure = 30;
            this.TouchSensorMode = TouchSensorStatusEnum.NotRequired;
            registrationSeq = (byte)((registrationSeq + 1) % 0xFF);
        }

        /// <summary>
        /// Gets or sets the registration sequence.
        /// </summary>
        /// <value>The registration sequence.</value>
        public byte RegistrationSequence { get; set; }

        /// <summary>
        /// Gets or sets the operation code.
        /// </summary>
        /// <value>The operation code.</value>
        public OpCodeCmds OperationCode { get; set; }

        /// <summary>
        /// Gets or sets the match number.
        /// </summary>
        /// <value>The match number. Range 1 - 255</value>
        public byte GameNumber { get; set; }

        /// <summary>
        /// Gets or sets the minimum pressure required for the device to report back a hit. Range 30-255.
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
}
