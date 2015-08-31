/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    public static class MeterEvents
    {
        public enum ResetMetersArgs
        {
            History,
            High,
            All
        }

        /// <summary>
        /// Fired to have all the meters reset
        /// </summary>
        public class ResetMeters : CompositePresentationEvent<ResetMetersArgs> { }

        /// <summary>
        /// Fired when the meter is removed from the display
        /// </summary>
        public class MeterRemoved : CompositePresentationEvent<MeterRemoved> 
        {
            public string SensorId { get; set; }
        }
    }

    public struct MeterDisplayNameChangedArgs
    {
        public string SensorId;
        public string NewMeterName;
    }

    /// <summary>
    /// Raised when a meter display name is changed
    /// </summary>
    public class OnMeterDisplayNameChangedEvent : CompositePresentationEvent<MeterDisplayNameChangedArgs> { }
}
