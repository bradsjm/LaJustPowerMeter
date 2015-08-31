// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeterViewModel.cs" company="LaJust Sports America">
//   All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Impact Meter View Model
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImpactMeter
{
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Infrastructure;

    using LaJust.EIDSS.Communications.Entities;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Impact Meter View Model
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MeterViewModel : NotificationObject
    {
        #region Constants and Fields

        /// <summary>
        /// Battery Low Indication
        /// </summary>
        private bool batteryLow;

        /// <summary>
        /// Device Registration Data for the device this meter represents
        /// </summary>
        private CompetitorModel competitor;

        /// <summary>
        /// Set to indicate if panels should be shown
        /// </summary>
        private bool hasPanels;

        /// <summary>
        /// Panel Number (for Targets) Struck
        /// </summary>
        private int hitPanelNum;

        /// <summary>
        /// Provides a status of active/inactive for this meter
        /// </summary>
        private bool isActive;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterViewModel"/> class.
        /// </summary>
        /// <param name="showOverlayRequest">
        /// The show overlay request.
        /// </param>
        [ImportingConstructor]
        public MeterViewModel(ApplicationEvents.ShowOverlay showOverlayRequest)
        {
            this.ShowPopupGraphCommand = new DelegateCommand(() => { });
            this.ShowPopupRosterCommand = new DelegateCommand(() => showOverlayRequest.Publish(PageNames.RosterPopUp));
            this.LevelIncreaseCommand = new DelegateCommand(() => this.Competitor.RequiredImpactLevel += 5);
            this.LevelDecreaseCommand = new DelegateCommand(() => this.Competitor.RequiredImpactLevel -= 5);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the battery is low.
        /// </summary>
        /// <value>The battery low indicator.</value>
        public bool BatteryLow
        {
            get
            {
                return this.batteryLow;
            }

            private set
            {
                this.batteryLow = value;
                this.RaisePropertyChanged(() => this.BatteryLow);
            }
        }

        /// <summary>
        /// Gets or sets the close meter command.
        /// </summary>
        /// <value>The close meter command.</value>
        public ICommand CloseMeterCommand { get; set; }

        /// <summary>
        /// Gets or sets the competitor that this meter represents.
        /// </summary>
        /// <value>The competitor.</value>
        public CompetitorModel Competitor
        {
            get
            {
                return this.competitor;
            }

            set
            {
                this.competitor = value;
                this.RaisePropertyChanged(() => this.Competitor);
                this.BatteryLow = false;
                this.competitor.OnChanged(c => c.DeviceStatus).Do(
                    delegate
                        {
                            this.BatteryLow = this.competitor.DeviceStatus == DeviceStatusEnum.LowBattery;
                            this.IsActive = this.competitor.DeviceStatus != DeviceStatusEnum.NotResponding;
                        });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has panels.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has panels; otherwise, <c>false</c>.
        /// </value>
        public bool HasPanels
        {
            get
            {
                return this.hasPanels;
            }

            set
            {
                this.hasPanels = value;
                this.RaisePropertyChanged(() => this.HasPanels);
            }
        }

        /// <summary>
        /// Gets or sets the hit panel num.
        /// </summary>
        /// <value>The hit panel num.</value>
        public int HitPanelNum
        {
            get
            {
                return this.hitPanelNum;
            }

            set
            {
                this.hitPanelNum = value;
                this.RaisePropertyChanged(() => this.HitPanelNum);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            private set
            {
                this.isActive = value;
                this.RaisePropertyChanged(() => this.IsActive);
            }
        }

        /// <summary>
        /// Gets the level decrease command.
        /// </summary>
        /// <value>The level decrease command.</value>
        public ICommand LevelDecreaseCommand { get; private set; }

        /// <summary>
        /// Gets the level increase command.
        /// </summary>
        /// <value>The level increase command.</value>
        public ICommand LevelIncreaseCommand { get; private set; }

        /// <summary>
        /// Gets or sets MoveMeterLeftCommand.
        /// </summary>
        public ICommand MoveMeterLeftCommand { get; set; }

        /// <summary>
        /// Gets or sets MoveMeterRightCommand.
        /// </summary>
        public ICommand MoveMeterRightCommand { get; set; }

        /// <summary>
        /// Gets the popup graph command.
        /// </summary>
        /// <value>The show popup graph command.</value>
        public ICommand ShowPopupGraphCommand { get; private set; }

        /// <summary>
        /// Gets the show popup roster command.
        /// </summary>
        /// <value>The show popup roster command.</value>
        public ICommand ShowPopupRosterCommand { get; private set; }

        #endregion
    }
}