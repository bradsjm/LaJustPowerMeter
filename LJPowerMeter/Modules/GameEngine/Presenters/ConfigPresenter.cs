/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.GameEngine.Presenters
{
    using System;
    using System.Windows.Input;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Models;
    using Microsoft.Practices.Composite.Presentation.Commands;
using LaJust.PowerMeter.Common.Helpers;

    public class ConfigPresenter : Presenter
    {
        #region Private Fields

        private readonly GameMetaDataModel _game;
        private readonly ConfigProperties _config;
        private readonly PropertyObserver<GameMetaDataModel> _gameObserver;

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the rounds per game.
        /// </summary>
        /// <value>The rounds per game.</value>
        public byte RoundsPerGame
        {
            get { return _config.RoundsPerGame; }
            private set
            {
                if (value >= 1 && value <= 5)
                {
                    _config.RoundsPerGame = value;
                    OnPropertyChanged("RoundsPerGame");
                }
            }
        }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber
        {
            get { return _game.GameNumber; }
            set { 
                _game.GameNumber = value;
                _game.RoundNumber = 1;
                OnPropertyChanged("GameNumber"); 
            }
        }

        #endregion

        #region Delegate Commands

        public ICommand RoundsPerGameIncreaseCommand { get; private set; }
        public ICommand RoundsPerGameDecreaseCommand { get; private set; }

        public ICommand GameNumberIncreaseCommand { get; private set; }
        public ICommand GameNumberDecreaseCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPresenter"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public ConfigPresenter(ConfigProperties config, GameMetaDataModel game)
        {
            _config = config;
            _game = game;
            InitializeCommands();
            _gameObserver = new PropertyObserver<GameMetaDataModel>(_game).RegisterHandler(o => o.GameNumber, o => OnPropertyChanged("GameNumber"));
        }

        #endregion

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        private void InitializeCommands()
        {
            RoundsPerGameIncreaseCommand = new DelegateCommand<string>(s => RoundsPerGame++);

            RoundsPerGameDecreaseCommand = new DelegateCommand<string>(s => RoundsPerGame--);

            GameNumberIncreaseCommand = new DelegateCommand<string>(s => GameNumber++);

            GameNumberDecreaseCommand = new DelegateCommand<string>(s => GameNumber--);
        }

    }
}
