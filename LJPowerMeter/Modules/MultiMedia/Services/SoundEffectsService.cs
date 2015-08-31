/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.MultiMedia.Services
{
    using System;
    using System.Collections.Generic;
    using System.Media;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using System.IO;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class SoundEffectsService
    {
        #region Constants

        private const string BUTTON_CLICK_WAV = "ButtonClick.wav";

        private const string SENSOR_CONNECTED_WAV = "Connected.wav";
        private const string SENSOR_LOST_WAV = "Disconnected.wav";

        private const string CLOCK_START_WAV = "StartTone.wav";
        private const string CLOCK_END_WAV = "EndTone.wav";
        private const string CLOCK_WARNING_WAV = "WarningTone.wav";

        private readonly string[] PUNCH_SOUNDS = { "Punch1.wav", "Punch2.wav", "Punch3.wav" };

        #endregion

        #region Private Fields

        private readonly SoundPlayer _soundPlayer = new SoundPlayer();
        private readonly ConfigProperties _config;
        private readonly Random _random = new Random();
        private readonly string _soundDirectory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SoundEffectsService(IEventAggregator eventAggregator, ConfigProperties config)
        {
            _config = config;
            _soundDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Sounds");
            SubscribeEvents(eventAggregator);
            RegisterMouseClick();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        /// <param name="aggregator">The aggregator.</param>
        private void SubscribeEvents(IEventAggregator eventAggregator)
        {
            // Sensor Events
            eventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Subscribe(OnDeviceRegistered);

            // Meter events
            eventAggregator.GetEvent<MeterEvents.MeterRemoved>().Subscribe(OnMeterRemoved);

            // Clock Events
            eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Subscribe(OnCountStateChanged);

            // Scoring Events
            eventAggregator.GetEvent<GameEngineEvents.ScoreChanged>().Subscribe(OnScoreChanged);
        }

        /// <summary>
        /// Registers the key click event.
        /// </summary>
        private void RegisterMouseClick()
        {
            Application.Current.MainWindow.PreviewMouseUp += MainWindow_MouseUp;
        }

        /// <summary>
        /// Plays the sound resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        private void PlaySoundResource(string soundFile)
        {
            lock (_soundPlayer)
            {
                _soundPlayer.Stop();
                _soundPlayer.SoundLocation = Path.Combine(_soundDirectory, soundFile);
                _soundPlayer.Play();
            };
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the MouseUp event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.Primitives.ButtonBase)
                PlaySoundResource(BUTTON_CLICK_WAV);
        }

        /// <summary>
        /// Called when [sensor registration].
        /// </summary>
        /// <param name="sensorRegistrationData">The sensor registration data.</param>
        private void OnDeviceRegistered(ReceiverEvents.DeviceRegistered e)
        {
            if (_config.MeterSoundsEnabled) PlaySoundResource(SENSOR_CONNECTED_WAV);
        }

        /// <summary>
        /// Called when [meter removed].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnMeterRemoved(MeterEvents.MeterRemoved e)
        {
            if (_config.MeterSoundsEnabled)
                PlaySoundResource(SENSOR_LOST_WAV);
        }

        /// <summary>
        /// Called when [sensor hit].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnScoreChanged(GameEngineEvents.ScoreChanged e)
        {
            if (_config.MeterSoundsEnabled && e.NewScore > e.OldScore)
                PlaySoundResource(PUNCH_SOUNDS[_random.Next(PUNCH_SOUNDS.Length)]);
        }

        /// <summary>
        /// Called when [count state changed].
        /// </summary>
        /// <param name="countDownState">State of the count down.</param>
        private void OnCountStateChanged(CountDownClockEvents.StateChangedArgs countDownState)
        {
            switch (countDownState)
            {
                case CountDownClockEvents.StateChangedArgs.Running: PlaySoundResource(CLOCK_START_WAV); break;
                case CountDownClockEvents.StateChangedArgs.Finished: PlaySoundResource(CLOCK_END_WAV); break;
            }
        }

        #endregion
    }
}