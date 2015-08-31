/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.MultiMedia.Services
{
    using System.Speech.Synthesis;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Events;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class SpeechService
    {
        #region Private Properties

        private readonly SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        private readonly IEventAggregator _eventAggregator;
        private readonly ConfigProperties _config;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SpeechService(IEventAggregator eventAggregator, ConfigProperties config)
        {
            _eventAggregator = eventAggregator;
            SubscribeEvents();
            _config = config;
            _speechSynthesizer.Volume = 100;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        /// <param name="aggregator">The aggregator.</param>
        private void SubscribeEvents()
        {
            // Sensor Events
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Subscribe(OnSensorHit, ThreadOption.BackgroundThread);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [sensor impact].
        /// </summary>
        /// <param name="sensorImpactData">The sensor impact data.</param>
        private void OnSensorHit(ReceiverEvents.SensorHit e)
        {
            if (_config.SpeakImpactLevelEnabled) lock (_speechSynthesizer)
            {
                _speechSynthesizer.SpeakAsyncCancelAll();
                _speechSynthesizer.SpeakAsync(e.ImpactLevel.ToString());
            }
        }

        #endregion
    }
}