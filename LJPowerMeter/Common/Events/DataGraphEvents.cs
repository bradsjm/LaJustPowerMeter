/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    /// <summary>
    /// Data Events
    /// </summary>
    public static class DataGraphEvents
    {
        /// <summary>
        /// Fired to invoke pop up from the Data Graph for specified sensor id
        /// </summary>
        public class ShowMeterHistory : CompositePresentationEvent<ShowMeterHistory>
        {
            public string SensorId { get; set; }
            public string DisplayName { get; set; }
            public byte GameNumber { get; set; }
            public byte RoundNumber { get; set; }
        }
    }
}
