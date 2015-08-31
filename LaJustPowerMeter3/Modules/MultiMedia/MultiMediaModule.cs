// <copyright file="MultiMediaModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace MultiMedia
{
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using Infrastructure;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Multi Media module handles providing audio for the application in response to published events
    /// </summary>
    [ModuleExport(typeof(MultiMediaModule))]
    public sealed class MultiMediaModule : IModule
    {
        /// <summary>
        /// Application configuration settings
        /// </summary>
        private readonly IAppConfigService appConfigService;

        private readonly IScoreKeeperService scoreKeeperService;

        private readonly IStopWatchService stopWatchService;

        /// <summary>
        /// Module sound effects service
        /// </summary>
        private readonly SoundEffectsService soundEffectsService;

        /// <summary>
        /// Module speach service
        /// </summary>
        private readonly SpeechService speachService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiMediaModule"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        [ImportingConstructor]
        public MultiMediaModule(
            IAppConfigService appConfigService,
            IScoreKeeperService scoreKeeperService,
            IStopWatchService stopWatchService)
        {
            this.appConfigService = appConfigService;
            this.scoreKeeperService = scoreKeeperService;
            this.stopWatchService = stopWatchService;

            this.soundEffectsService = new SoundEffectsService();
            this.speachService = new SpeechService();
        }

        /// <summary>
        /// Notifies the module to initialize.
        /// </summary>
        public void Initialize()
        {
            this.WireUpSoundEffectHandlers();
            this.WireUpSpeachHandlers();
        }

        /// <summary>
        /// Wires up sound effect handlers.
        /// </summary>
        private void WireUpSoundEffectHandlers()
        {
            // Mouse Clicks
            //Application.Current.MainWindow.PreviewMouseUp += this.soundEffectsService.MainWindow_MouseUp;

            // Chronograph Start/Stop events
            this.stopWatchService.OnChanged(p => p.IsRunning).Do(delegate
            {
                this.soundEffectsService.PlaySoundFile(
                    this.stopWatchService.IsRunning ? SoundEffectsService.CLOCK_START_WAV : SoundEffectsService.CLOCK_END_WAV);
            });
            
            // Meter add/remove events
            this.scoreKeeperService.Competitors.CollectionChanged += (s, e) =>
            {
                if (this.appConfigService.Settings.MeterSoundsEnabled)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            this.soundEffectsService.PlaySoundFile(SoundEffectsService.SENSOR_CONNECTED_WAV);
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            this.soundEffectsService.PlaySoundFile(SoundEffectsService.SENSOR_LOST_WAV);
                            break;
                    }
                }
            };

            // Score change events
            this.scoreKeeperService.Competitors.ChildElementPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HighestImpactLevel" && this.appConfigService.Settings.MeterSoundsEnabled)
                {
                    var level = ((CompetitorModel)e.ChildElement).HighestImpactLevel;
                    if (level > 0) this.soundEffectsService.PlaySoundFile(SoundEffectsService.UGH_SOUND);
                }

                else if (e.PropertyName == "Score" && this.appConfigService.Settings.MeterSoundsEnabled && this.stopWatchService.IsRunning)
                {
                    this.soundEffectsService.PlaySoundFile(SoundEffectsService.HIT_SOUNDS);
                }
            };
        }

        /// <summary>
        /// Wires up speach handlers.
        /// </summary>
        private void WireUpSpeachHandlers()
        {
            this.scoreKeeperService.Competitors.ChildElementPropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "LastImpactLevel" && this.appConfigService.Settings.SpeakImpactLevelEnabled)
                {
                    var value = ((CompetitorModel)e.ChildElement).LastImpactLevel;
                    if (value > 0) this.speachService.SpeakText(value.ToString());
                }
            };
        }
    }
}
