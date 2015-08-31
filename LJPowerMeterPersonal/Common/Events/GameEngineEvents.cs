namespace LaJust.PowerMeter.Common.Events
{
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    public static class GameEngineEvents
    {
        /// <summary>
        /// Raised when the score changes
        /// </summary>
        public class ScoreChanged : CompositePresentationEvent<ScoreChanged>
        {
            public string SensorId { get; set; }
            public byte ImpactLevel { get; set; }
            public uint OldScore { get; set; }
            public uint NewScore { get; set;}
        }
    }
}
