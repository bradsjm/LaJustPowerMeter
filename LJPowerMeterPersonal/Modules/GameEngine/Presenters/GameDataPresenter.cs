/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.GameEngine.Presenters
{
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Models;
    using Microsoft.Practices.Unity;

    public class GameDataPresenter : Presenter
    {
        private readonly GameMetaDataModel _gameMetaData;

        /// <summary>
        /// Gets the game meta data model.
        /// </summary>
        /// <value>The game meta data model.</value>
        public GameMetaDataModel Game
        {
            get { return _gameMetaData; }
        }

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRoundPresenter"/> class.
        /// </summary>
        public GameDataPresenter(GameMetaDataModel gameMetaData)
        {
            _gameMetaData = gameMetaData;
        }

        #endregion
    }
}
