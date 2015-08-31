namespace LaJust.PowerMeter.Common
{
    using System;
    using LaJust.PowerMeter.Common.BaseClasses;

    /// <summary>
    /// List of available configuration properties
    /// </summary>
    public class ConfigProperties : ConfigBase
    {
        #region Private Backing Fields

        private bool _contactSensorRequired;
        private byte _courtNumber;
        private bool _meterSoundsEnabled;
        private byte _requiredImpactLevel;
        private TimeSpan _roundDuration;
        private byte _roundsPerGame;
        private bool _speakImpactLevelEnabled;

        #endregion

        #region Public Properties

        public bool ContactSensorRequired { get { return _contactSensorRequired; } set { _contactSensorRequired = value; OnPropertyChanged("ContactSensorRequired"); } }

        public byte CourtNumber { get { return _courtNumber; } set { _courtNumber = value; OnPropertyChanged("CourtNumber"); } }

        public bool MeterSoundsEnabled { get { return _meterSoundsEnabled; } set { _meterSoundsEnabled = value; OnPropertyChanged("MeterSoundsEnabled"); } }

        public byte RequiredImpactLevel { get { return _requiredImpactLevel; } set { _requiredImpactLevel = Math.Max((byte)10, value); OnPropertyChanged("RequiredImpactLevel"); } }

        public TimeSpan RoundDuration { get { return _roundDuration; } set { _roundDuration = value; OnPropertyChanged("RoundDuration"); } }

        public byte RoundsPerGame { get { return _roundsPerGame; } set { _roundsPerGame = Math.Min((byte)5, value); OnPropertyChanged("RoundsPerGame"); } }

        public bool SpeakImpactLevelEnabled { get { return _speakImpactLevelEnabled; } set { _speakImpactLevelEnabled = value; OnPropertyChanged("SpeakImpactLevelEnabled"); } }

        #endregion
    }
}
