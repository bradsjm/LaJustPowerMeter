/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    public static class CountDownClockEvents
    {
        public enum ChangeStateArgs
        {
            Start,
            Pause,
            Reset
        }

        /// <summary>
        /// Published to request a change in the state of the count down clock
        /// </summary>
        public class ChangeState : CompositePresentationEvent<ChangeStateArgs> { }

        public enum StateChangedArgs
        {
            Running,
            Paused,
            Finished,
            Reset
        }

        /// <summary>
        /// Published by count down clock when state has changed
        /// </summary>
        public class StateChanged : CompositePresentationEvent<StateChangedArgs> { }
    }
}
