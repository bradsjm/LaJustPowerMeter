namespace LaJust.PowerMeter.Common.Models
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;

    /// <summary>
    /// Simple model for a "Score" that has an owner and a value
    /// </summary>
    public class GameMetaDataModel : PropertyNotifier
    {
        private const byte MAX_ROUNDS = 5;

        private byte _gameNumber = 1;
        private byte _roundNumber = 1;

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber
        {
            get { return _gameNumber; }
            set { _gameNumber = Math.Max((byte)1, value); OnPropertyChanged("GameNumber"); }
        }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public byte RoundNumber
        {
            get { return _roundNumber; }
            set { _roundNumber = Math.Min(MAX_ROUNDS, Math.Max((byte)1, value)); OnPropertyChanged("RoundNumber"); }
        }
    }
}
