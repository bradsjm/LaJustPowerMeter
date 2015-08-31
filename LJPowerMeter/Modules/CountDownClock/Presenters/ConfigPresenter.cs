/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.CountDownClock.Presenters
{
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using System.Windows.Input;
    using System;
    using LaJust.PowerMeter.Common.Events;

    public class ConfigPresenter : Presenter
    {
        #region Private Fields

        private readonly ConfigProperties _config;

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the rounds per game.
        /// </summary>
        /// <value>The rounds per game.</value>
        public TimeSpan GameRoundDuration
        {
            get { return _config.RoundDuration; }
            set
            {
                if (value > TimeSpan.Zero)
                    _config.RoundDuration = value;
                else
                    _config.RoundDuration = TimeSpan.Zero;
                OnPropertyChanged("GameRoundDuration");
            }
        }
        
        #endregion

        #region Delegate Commands

        public ICommand GameRoundDurationIncreaseCommand { get; private set; }
        public ICommand GameRoundDurationDecreaseCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPresenter"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public ConfigPresenter(ConfigProperties config)
        {
            _config = config;
            InitializeCommands();
        }

        #endregion

        #region Private Implementation Methods

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            GameRoundDurationIncreaseCommand = new DelegateCommand<string>(
                s => GameRoundDuration += TimeSpan.FromSeconds(30));

            GameRoundDurationDecreaseCommand = new DelegateCommand<string>(
                s => GameRoundDuration -= TimeSpan.FromSeconds(30));
        }

        #endregion

    }
}
