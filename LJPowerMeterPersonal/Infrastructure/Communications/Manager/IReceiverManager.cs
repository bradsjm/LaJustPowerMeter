/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
using System;
namespace LaJust.EIDSS.Communications
{
    public interface IReceiverManager
    {
        int Count();
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceRegistrationEventData> DeviceRegistered;
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceStatusEventData> DeviceStatusUpdate;
        void Dispose();
        System.Collections.ObjectModel.ReadOnlyCollection<LaJust.EIDSS.Communications.Hardware.IReceiver> GetReceivers();
        event EventHandler<LaJust.EIDSS.Communications.Entities.PanelButtonEventData> PanelButtonPressed;
        event EventHandler<LaJust.EIDSS.Communications.ReceiverCountEventArgs> ReceiverCountChanged;
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceEventData> StrikeDetected;
    }
}
