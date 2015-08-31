/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.MultiMedia.Presenters
{
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using System;
    using System.Windows.Input;

    public class ConfigPresenter : Presenter
    {
        #region Private Fields

        private readonly ConfigProperties _config;

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets a value indicating whether [sound effects enabled].
        /// </summary>
        /// <value><c>true</c> if [sound effects enabled]; otherwise, <c>false</c>.</value>
        public bool SoundEffectsEnabled
        {
            get { return _config.MeterSoundsEnabled; }
            set
            {
                _config.MeterSoundsEnabled = value;
                OnPropertyChanged("SoundEffectsEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [speak impact level].
        /// </summary>
        /// <value><c>true</c> if [speak impact level]; otherwise, <c>false</c>.</value>
        public bool SpeakImpactLevelEnabled
        {
            get { return _config.SpeakImpactLevelEnabled; }
            set
            {
                _config.SpeakImpactLevelEnabled = value;
                OnPropertyChanged("SpeakImpactLevelEnabled");
            }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPresenter"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public ConfigPresenter(ConfigProperties config)
        {
            _config = config;
        }

        #endregion

    }
}
