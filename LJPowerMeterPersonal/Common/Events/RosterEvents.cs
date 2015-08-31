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
    public static class RosterEvents
    {
        /// <summary>
        /// Fired to invoke pop up from the Roster to pick a name
        /// </summary>
        public class ShowRosterPickList : CompositePresentationEvent<ShowRosterPickList>
        {
            /// <summary>
            /// Gets or sets the result handler which contains the selected name from the roster
            /// </summary>
            /// <value>The result handler.</value>
            public Action<string> ResultHandler { get; set; }
        }
    }
}
