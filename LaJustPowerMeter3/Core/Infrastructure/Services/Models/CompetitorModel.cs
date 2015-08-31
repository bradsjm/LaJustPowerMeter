// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompetitorModel.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Model for a competitor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Xml.Serialization;

    using LaJust.EIDSS.Communications.Entities;

    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Model for a competitor
    /// </summary>
    [Serializable]
    public class CompetitorModel : NotificationObject
    {
        #region Constants and Fields

        /// <summary>
        /// History of impacts since reset
        /// </summary>
        private readonly DispatchingObservableCollection<byte> impactHistory =
            new DispatchingObservableCollection<byte>();

        /// <summary>
        /// The owner of this instance
        /// </summary>
        private DeviceId deviceId;

        /// <summary>
        /// Status of the device being worn by the competitor
        /// </summary>
        private DeviceStatusEnum deviceStatus = DeviceStatusEnum.Invalid;

        /// <summary>
        /// Reference to the device type used/worn by the competitor
        /// </summary>
        private OpCodes deviceType = OpCodes.Invalid;

        /// <summary>
        /// The Display Name of the competitor
        /// </summary>
        private string displayName = "Unknown";

        /// <summary>
        /// The Highest Impact Level recorded since reset
        /// </summary>
        private byte highestImpactLevel;

        /// <summary>
        /// The last impact level recorded
        /// </summary>
        private byte lastImpactLevel;

        /// <summary>
        /// The partner (if any)
        /// </summary>
        private CompetitorModel partner;

        /// <summary>
        /// The penalty value
        /// </summary>
        private uint penaltyValue;

        /// <summary>
        /// Device Registration Data
        /// </summary>
        private DeviceRegistrationEventArgs registration;

        /// <summary>
        /// The required impact level (to score)
        /// </summary>
        private byte requiredImpactLevel = 30;

        /// <summary>
        /// The score value
        /// </summary>
        private uint scoreValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitorModel"/> class.
        /// </summary>
        public CompetitorModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitorModel"/> class,
        /// takes a registration instance and populates the device Id, Device Type, 
        /// default name and required impact level fields from the data.
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        public CompetitorModel(DeviceRegistrationEventArgs registration)
        {
            this.registration = registration;
            this.DeviceId = this.registration.Id;
            this.DeviceType = this.registration.OperationCode;
            this.RequiredImpactLevel = this.registration.MinimumPressure;
            this.DisplayName = GetDefaultDisplayName(this.registration.OperationCode);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the device Id owner of this instance
        /// </summary>
        /// <value>The instance owner device id.</value>
        [XmlElement]
        public DeviceId DeviceId
        {
            get
            {
                return this.deviceId;
            }

            set
            {
                this.deviceId = value;
                this.RaisePropertyChanged(() => this.DeviceId);
            }
        }

        /// <summary>
        /// Gets or sets the device registration information.
        /// </summary>
        /// <value>The device registration.</value>
        [XmlElement]
        public DeviceRegistrationEventArgs DeviceRegistration
        {
            get
            {
                return this.registration;
            }

            set
            {
                this.registration = value;
                this.RaisePropertyChanged(() => this.DeviceRegistration);
            }
        }

        /// <summary>
        /// Gets or sets the status of the device being worn/used by the competitor
        /// </summary>
        /// <value>The device status.</value>
        [XmlElement]
        public DeviceStatusEnum DeviceStatus
        {
            get
            {
                return this.deviceStatus;
            }

            set
            {
                this.deviceStatus = value;
                this.RaisePropertyChanged(() => this.DeviceStatus);
            }
        }

        /// <summary>
        /// Gets or sets the type of the device worn/used by the competitor.
        /// </summary>
        /// <value>The type of the device.</value>
        [XmlElement]
        public OpCodes DeviceType
        {
            get
            {
                return this.deviceType;
            }

            set
            {
                this.deviceType = value;
                this.RaisePropertyChanged(() => this.DeviceType);
            }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [XmlElement]
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.displayName = value;
                this.RaisePropertyChanged(() => this.DisplayName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a partner.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has partner; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool HasPartner
        {
            get
            {
                return this.partner != null;
            }
        }

        /// <summary>
        /// Gets the highest impact level.
        /// </summary>
        /// <value>The highest impact level.</value>
        [XmlIgnore]
        public byte HighestImpactLevel
        {
            get
            {
                return this.highestImpactLevel;
            }

            private set
            {
                this.highestImpactLevel = value;
                this.RaisePropertyChanged(() => this.HighestImpactLevel);
            }
        }

        /// <summary>
        /// Gets the impact history since the last reset.
        /// </summary>
        /// <value>The impact history.</value>
        [XmlIgnore]
        public DispatchingObservableCollection<byte> ImpactHistory
        {
            get
            {
                return this.impactHistory;
            }
        }

        /// <summary>
        /// Gets or sets the last impact level.
        /// </summary>
        /// <value>The last impact level.</value>
        [XmlIgnore]
        public byte LastImpactLevel
        {
            get
            {
                return this.lastImpactLevel;
            }

            set
            {
                this.lastImpactLevel = value;
                this.ImpactHistory.Add(value);
                if (value > this.HighestImpactLevel)
                {
                    this.HighestImpactLevel = value;
                }

                this.RaisePropertyChanged(() => this.LastImpactLevel);
            }
        }

        /// <summary>
        /// Gets or sets the partner (if any). Set to null if no partner.
        /// </summary>
        /// <value>The partner.</value>
        [XmlIgnore]
        public CompetitorModel Partner
        {
            get
            {
                return this.partner;
            }

            set
            {
                this.partner = value;
                this.RaisePropertyChanged(() => this.Partner);
                this.RaisePropertyChanged(() => this.HasPartner);
                if (value != null && value.Partner != this)
                {
                    value.Partner = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the penalties.
        /// </summary>
        /// <value>The penalties.</value>
        [XmlElement]
        public uint Penalties
        {
            get
            {
                return this.penaltyValue;
            }

            set
            {
                this.penaltyValue = value;
                this.RaisePropertyChanged(() => this.Penalties);
            }
        }

        /// <summary>
        /// Gets or sets the required impact level.
        /// </summary>
        /// <value>The required impact level.</value>
        [XmlElement]
        public byte RequiredImpactLevel
        {
            get
            {
                return this.requiredImpactLevel;
            }

            set
            {
                if (this.registration != null && this.registration.MinimumPressure <= value)
                {
                    this.requiredImpactLevel = value;
                    this.RaisePropertyChanged(() => this.RequiredImpactLevel);
                }
            }
        }

        /// <summary>
        /// Gets or sets the score value of this instance.
        /// </summary>
        /// <value>The score.</value>
        [XmlElement]
        public uint Score
        {
            get
            {
                return this.scoreValue;
            }

            set
            {
                this.scoreValue = value;
                this.RaisePropertyChanged(() => this.Score);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the meta data on this competitor for a new round
        /// </summary>
        public void Reset()
        {
            this.Score = 0;
            this.LastImpactLevel = 0;
            this.HighestImpactLevel = 0;
            this.Penalties = 0;
            this.ImpactHistory.Clear();
        }

        /// <summary>
        /// Removes partnership if it exists from both competitors. This is important to do
        /// so we don't have hanging references
        /// </summary>
        public void UnPartner()
        {
            if (this.HasPartner)
            {
                this.Partner.Partner = null;
                this.Partner = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default name of the device for display (Hong, Chung etc.)
        /// </summary>
        /// <param name="operationCode">
        /// The op code.
        /// </param>
        /// <returns>
        /// A display name
        /// </returns>
        protected static string GetDefaultDisplayName(OpCodes operationCode)
        {
            switch (operationCode)
            {
                case OpCodes.HongRegistered:
                case OpCodes.HongPreRegistered:
                    return "Hong";

                case OpCodes.ChungRegistered:
                case OpCodes.ChungPreRegistered:
                    return "Chung";

                case OpCodes.TargetRegistered:
                    return "Target";
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}