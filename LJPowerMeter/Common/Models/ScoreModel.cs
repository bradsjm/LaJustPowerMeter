namespace LaJust.PowerMeter.Common.Models
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;

    /// <summary>
    /// Simple model for a "Score" that has an owner and a value
    /// </summary>
    public class ScoreModel : PropertyNotifier
    {
        private uint _scoreValue = 0;
        private string _owner = string.Empty;

        /// <summary>
        /// Gets or sets the score value of this instance.
        /// </summary>
        /// <value>The score.</value>
        public uint Value
        {
            get { return _scoreValue; }
            set { _scoreValue = value; OnPropertyChanged("Value"); }
        }

        /// <summary>
        /// Gets or sets the owner of this score instance.
        /// </summary>
        /// <value>The owner.</value>
        public string Owner
        {
            get { return _owner; }
            set { _owner = value; OnPropertyChanged("Owner"); }
        }

        /// <summary>
        /// Clears the value of this instance
        /// </summary>
        public void Reset()
        {
            this.Value = 0;
        }
    }
}
