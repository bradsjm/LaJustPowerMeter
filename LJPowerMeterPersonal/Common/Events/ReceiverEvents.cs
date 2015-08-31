/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    /// <summary>
    /// Receiver Events from hardware devices
    /// </summary>
    public static class ReceiverEvents
    {
        #region Public Enumerations

        /// <summary>
        /// Receiver Panel Buttons
        /// </summary>
        public enum PanelButtons
        {
            Unknown,
            Start,
            Clocking,
            TimeLimit,
            ChungWin,
            HongWin,
            ChungRegister,
            HongRegister,
            ChungScoreMinus,
            ChungScorePlus,
            ChungWarningMinus,
            ChungWarningPlus,
            HongScoreMinus,
            HongScorePlus,
            HongWarningMinus,
            HongWarningPlus,
            ChungTimeLimit,
            HongTimeLimit
        }

        /// <summary>
        /// Panels for devices that report which panel was hit
        /// </summary>
        public enum SensorPanel
        {
            Unknown,
            TopLeft,
            TopMiddle,
            TopRight,
            BottomLeft,
            BottomMiddle,
            BottomRight
        }

        /// <summary>
        /// Type of sensor device
        /// </summary>
        public enum SensorDeviceType
        {
            Unknown,
            Chung,
            Hong,
            Target
        }

        public enum SensorDeviceStatus
        {
            HoguOk,
            TargetOk,
            NotResponding,
            LowBattery
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fired when a button is pressed on the receiver panel
        /// </summary>
        public class ButtonPressed : CompositePresentationEvent<PanelButtons> { }

        /// <summary>
        /// Fired when a sensor device reports a hit
        /// </summary>
        public class SensorHit : CompositePresentationEvent<SensorHit>
        {
            public string SensorId { get; set; }
            public string OpCode { get; set; }
            public byte ImpactLevel { get; set; }
            public SensorPanel Panel { get; set; }
        }

        /// <summary>
        /// Fired when a device (Hogu, Target etc.) is registered
        /// </summary>
        public class DeviceRegistered : CompositePresentationEvent<DeviceRegistered>
        {
            public string SensorId { get; set; }
            public SensorDeviceType SensorType { get; set; }
        }

        /// <summary>
        /// Fired when a device (Hogu, Target etc.) provides status information
        /// </summary>
        public class DeviceStatusUpdate : CompositePresentationEvent<DeviceStatusUpdate> 
        {
            public string SensorId { get; set; }
            public SensorDeviceStatus Status { get; set; }
        }

        #endregion
    }
}
