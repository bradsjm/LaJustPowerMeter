/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
using System;
using LaJust.EIDSS.Communications.Entities;
using System.Collections.ObjectModel;

namespace LaJust.EIDSS.Communications.Hardware
{
    public interface IReceiver
    {
        ReadOnlyCollection<DeviceRegistrationEventData> GetDeviceRegistrations();
        void ClearGameRegistration(byte GameNumber);
        void ClearGameRegistrations();
        byte CourtNumber { get; }
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceStatusEventData> DeviceStatusUpdate;
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceRegistrationEventData> DeviceRegistered;
        void Dispose();
        void GenerateTone(LaJust.EIDSS.Communications.Entities.ToneTypeEnum toneType);
        string Id { get; }
        event EventHandler<LaJust.EIDSS.Communications.Entities.PanelButtonEventData> PanelButtonPressed;
        void PreRegisterDevice(LaJust.EIDSS.Communications.Entities.PreRegistrationSettings registration);
        void RegisterDevice(LaJust.EIDSS.Communications.Entities.RegistrationSettings registration);
        #if DEBUG
        void SendDebugBytes(byte[] data);
        #endif
        event EventHandler<LaJust.EIDSS.Communications.Entities.DeviceEventData> StrikeDetected;
    }
}
