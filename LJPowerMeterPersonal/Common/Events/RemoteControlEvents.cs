/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    #region Remote Control Button Press Event

    public static class RemoteEvents
    {
        /// <summary>
        /// Remote Buttons
        /// </summary>
        public enum Buttons
        {
            Unknown,
            Start,
            Stop,
            Right,
            Left,
            RegisterTarget,
            RegisterChung,
            RegisterHong
        }

        /// <summary>
        /// Fired when a button is pressed on the remote
        /// </summary>
        public class ButtonPressed : CompositePresentationEvent<Buttons> { }
    }
    #endregion
}
