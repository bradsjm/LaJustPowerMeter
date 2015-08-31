// <copyright file="IReceiverManager.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace LaJust.EIDSS.Communications
{
    using System;

    /// <summary>
    /// Interface for Receiver Manager
    /// </summary>
    public interface IReceiverManager
    {
        /// <summary>
        /// Occurs when [panel button pressed].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.PanelButtonEventData> PanelButtonPressed;

        /// <summary>
        /// Occurs when [receiver count changed].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.ReceiverCountEventArgs> ReceiverCountChanged;

        /// <summary>
        /// Occurs when [strike detected].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceDataEventArgs> StrikeDetected;

        /// <summary>
        /// Occurs when [device registered].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceRegistrationEventArgs> DeviceRegistered;

        /// <summary>
        /// Occurs when [device status update].
        /// </summary>
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceStatusEventArgs> DeviceStatusUpdate;

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>Count of connected receivers</returns>
        int Count();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        void Dispose();

        /// <summary>
        /// Resets all receivers.
        /// </summary>
        void ResetAll();

        /// <summary>
        /// Gets the receivers.
        /// </summary>
        /// <returns>Read only collection of receiver interfaces</returns>
        System.Collections.ObjectModel.ReadOnlyCollection<LaJust.EIDSS.Communications.Hardware.IReceiver> GetReceivers();
    }
}
