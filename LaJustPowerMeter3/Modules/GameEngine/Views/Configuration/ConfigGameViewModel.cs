// <copyright file="ConfigGameViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using Infrastructure;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.ViewModel;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Configuration Screen Game Number View Model
    /// </summary>
    [Export(typeof(ConfigGameViewModel))]
    public class ConfigGameViewModel : NotificationObject
    {
        #region Private Fields

        /// <summary>
        /// The Application Configuration
        /// </summary>
        protected readonly AppSettingsModel ApplicationConfiguration;

        /// <summary>
        /// The Score Keeper Service
        /// </summary>
        protected readonly IScoreKeeperService ScoreKeeper;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigGameNumberViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configService">The config service.</param>
        /// <param name="scoreService">The score service.</param>
        [ImportingConstructor]
        public ConfigGameViewModel(IEventAggregator eventAggregator, ILoggerFacade logger, IScoreKeeperService scoreService)
        {
            this.ScoreKeeper = scoreService;
            logger.Log("ConfigGameNumberViewModel Initialized", Category.Debug, Priority.None);
        }

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the rounds per game.
        /// </summary>
        /// <value>The rounds per game.</value>
        public byte RoundsPerGame
        {
            get 
            {
                return this.ApplicationConfiguration.RoundsPerGame; 
            }

            set
            {
                if (value >= 1 && value <= 5)
                {
                    this.ApplicationConfiguration.RoundsPerGame = value;
                    RaisePropertyChanged( () => this.RoundsPerGame );
                }
            }
        }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber
        {
            get 
            {
                return this.ScoreKeeper.GameNumber; 
            }

            set
            {
                this.ScoreKeeper.GameNumber = value;
                RaisePropertyChanged( () => this.GameNumber );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [contact sensor required].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [contact sensor required]; otherwise, <c>false</c>.
        /// </value>
        public bool ContactSensorRequired
        {
            get
            {
                return this.ApplicationConfiguration.ContactSensorRequired;
            }

            set
            {
                this.ApplicationConfiguration.ContactSensorRequired = value;
                RaisePropertyChanged( () => this.ContactSensorRequired );
            }
        }

        /// <summary>
        /// Gets or sets the required impact level.
        /// </summary>
        /// <value>The required impact level.</value>
        public byte RequiredImpactLevel
        {
            get
            {
                return this.ApplicationConfiguration.RequiredImpactLevel;
            }

            set
            {
                this.ApplicationConfiguration.RequiredImpactLevel = value;
                RaisePropertyChanged( () => this.RequiredImpactLevel );
            }
        }

        #endregion
    }
}
