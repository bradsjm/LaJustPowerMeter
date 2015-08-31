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
    public static class ScreenEvents
    {
        /// <summary>
        /// Fired to invoke pop up from the Roster to pick a name
        /// </summary>
        public class ActiveScreenChanged : CompositePresentationEvent<ActiveScreenChanged>
        {
            /// <summary>
            /// Gets or sets the name of the region.
            /// </summary>
            /// <value>The name of the region.</value>
            public string RegionName { get; set; }

            /// <summary>
            /// Gets or sets the name of the page.
            /// </summary>
            /// <value>The name of the page.</value>
            public PageNames PageName { get; set; }
        }
    }
}
