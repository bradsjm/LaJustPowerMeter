// <copyright file="IReceiver.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications.Hardware
{
    using System;

    /// <summary>
    /// Interface for EIDSS Receiver
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Occurs when [device registered].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceRegistrationEventArgs> DeviceRegistered;

        /// <summary>
        /// Occurs when [device status update].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceStatusEventArgs> DeviceStatusUpdate;

        /// <summary>
        /// Occurs when [strike detected].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceDataEventArgs> StrikeDetected;

        /// <summary>
        /// Occurs when [panel button pressed].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.PanelButtonEventData> PanelButtonPressed;

        /// <summary>
        /// Gets the court number.
        /// </summary>
        /// <value>The court number.</value>
        byte CourtNumber { get; }

        /// <summary>
        /// Gets the id for the receiver object.
        /// </summary>
        /// <value>The receiver object id.</value>
        string Id { get; }

        /// <summary>
        /// Clears the game registration.
        /// </summary>
        /// <param name="gameNumber">The game number.</param>
        void ClearGameRegistration(byte gameNumber);

        /// <summary>
        /// Clears all the game registrations.
        /// </summary>
        void ClearGameRegistrations();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        void Dispose();

        /// <summary>
        /// Generates the tone.
        /// </summary>
        /// <param name="toneType">Type of the tone.</param>
        void GenerateTone(LaJust.EIDSS.Communications.Entities.ToneTypeEnum toneType);

        /// <summary>
        /// Gets the device registrations.
        /// </summary>
        /// <returns>ReadOnlyCollection of device registrations</returns>
        System.Collections.ObjectModel.ReadOnlyCollection<LaJust.EIDSS.Communications.Entities.DeviceRegistrationEventArgs> GetDeviceRegistrations();

        /// <summary>
        /// Pres the register device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        void PreRegisterDevice(LaJust.EIDSS.Communications.Entities.PreRegistrationSettings registration);

        /// <summary>
        /// Registers the device.
        /// </summary>
        /// <param name="registration">The registration.</param>
        void RegisterDevice(LaJust.EIDSS.Communications.Entities.RegistrationSettings registration);

        /// <summary>
        /// Sends the debug bytes.
        /// </summary>
        /// <param name="dataPacket">The data packet.</param>
        void SendDebugBytes(byte[] dataPacket);
    }
}
