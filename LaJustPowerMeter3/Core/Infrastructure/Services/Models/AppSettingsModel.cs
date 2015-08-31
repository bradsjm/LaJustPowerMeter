// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsModel.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Application Configuration Model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;

    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Application Configuration Model
    /// </summary>
    public class AppSettingsModel : NotificationObject
    {
        #region Constants and Fields

        /// <summary>
        /// Contact sensor is required
        /// </summary>
        private bool contactSensorRequired;

        /// <summary>
        /// Starting Court Number to use (incremented per receiver)
        /// </summary>
        private byte courtNumber = 1;

        /// <summary>
        /// Meter Sound Effects Enabled
        /// </summary>
        private bool meterSoundsEnabled = true;

        /// <summary>
        /// Required Impact Level for Scoring
        /// </summary>
        private byte requiredImpactLevel = 30;

        /// <summary>
        /// Duration of each round
        /// </summary>
        private TimeSpan roundDuration = TimeSpan.FromMinutes(3);

        /// <summary>
        /// Number of rounds per game
        /// </summary>
        private byte roundsPerGame = 1;

        /// <summary>
        /// Enable speaking of impact levels
        /// </summary>
        private bool speakImpactLevelEnabled = true;

        #endregion

        #region Properties

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
                return this.contactSensorRequired;
            }

            set
            {
                this.contactSensorRequired = value;
                this.RaisePropertyChanged(() => this.ContactSensorRequired);
            }
        }

        /// <summary>
        /// Gets or sets the court number.
        /// </summary>
        /// <value>The court number.</value>
        public byte CourtNumber
        {
            get
            {
                return this.courtNumber;
            }

            set
            {
                this.courtNumber = value;
                this.RaisePropertyChanged(() => this.CourtNumber);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [meter sounds enabled].
        /// </summary>
        /// <value><c>true</c> if [meter sounds enabled]; otherwise, <c>false</c>.</value>
        public bool MeterSoundsEnabled
        {
            get
            {
                return this.meterSoundsEnabled;
            }

            set
            {
                this.meterSoundsEnabled = value;
                this.RaisePropertyChanged(() => this.MeterSoundsEnabled);
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
                return this.requiredImpactLevel;
            }

            set
            {
                this.requiredImpactLevel = Math.Max((byte)10, value);
                this.RaisePropertyChanged(() => this.RequiredImpactLevel);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the round.
        /// </summary>
        /// <value>The duration of the round.</value>
        public TimeSpan RoundDuration
        {
            get
            {
                return this.roundDuration;
            }

            set
            {
                this.roundDuration = value;
                this.RaisePropertyChanged(() => this.RoundDuration);
            }
        }

        /// <summary>
        /// Gets or sets the rounds per game.
        /// </summary>
        /// <value>The rounds per game.</value>
        public byte RoundsPerGame
        {
            get
            {
                return this.roundsPerGame;
            }

            set
            {
                this.roundsPerGame = Math.Min((byte)5, value);
                this.RaisePropertyChanged(() => this.RoundsPerGame);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [speak impact level enabled].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [speak impact level enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool SpeakImpactLevelEnabled
        {
            get
            {
                return this.speakImpactLevelEnabled;
            }

            set
            {
                this.speakImpactLevelEnabled = value;
                this.RaisePropertyChanged(() => this.SpeakImpactLevelEnabled);
            }
        }

        #endregion
    }
}