/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Meter.Presenters
{
    using System;
    using System.Windows.Input;
    using System.Windows.Media;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Common.Extensions;
    using LaJust.PowerMeter.Common.Models;
    using LaJust.PowerMeter.ControlLibrary.SimpleGraph;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;

    public class MeterPresenter : Presenter
    {
        #region Private Fields

        private IEventAggregator _eventAggregator;
        private IUnityContainer _container;
        private int _hitPanelNum = 0;
        private string _id = String.Empty;
        private bool _isActive = true;

        private byte _impactLevel = 0;
        private byte _highestImpactLevel = 0;
        private string _displayName = string.Empty;
        private GraphDataCollection _graphDataItems = new GraphDataCollection();
        private bool _batteryLow = false;
        private ReceiverEvents.SensorDeviceType _meterType;
        private ConfigProperties _config;
        private Brush _meterColor;  //TODO: This should be a string and use a XAML converter
        private bool _hasPanels;
        private byte _targetNumber;
        private static byte _targetCount;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the internal identifier (key value) for this presenter.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); OnPropertyChanged("Score"); }
        }

        /// <summary>
        /// Gets the score model.
        /// </summary>
        /// <value>The score.</value>
        public ScoreModel Score
        {
            get 
            {
                if (!_container.CanResolve<ScoreModel>(this.Id))
                    _container.RegisterInstance<ScoreModel>(this.Id, new ScoreModel() { Owner = this.Id });
                return _container.Resolve<ScoreModel>(this.Id);
            }
        }

        /// <summary>
        /// Gets or sets the name of the meter.
        /// </summary>
        /// <value>The name of the identifier.</value>
        public string DisplayName
        {
            get { return _displayName; }
            set 
            {
                _displayName = value; 
                OnPropertyChanged("DisplayName");
                _eventAggregator.GetEvent<OnMeterDisplayNameChangedEvent>().Publish(
                    new MeterDisplayNameChangedArgs() { SensorId = this.Id, NewMeterName = value });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return _isActive; }
            private set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        /// <summary>
        /// Gets the impact level
        /// </summary>
        /// <value>The impact level.</value>
        public byte ImpactLevel
        {
            get { return _impactLevel; }
            private set { _impactLevel = value; OnPropertyChanged("ImpactLevel"); }
        }

        /// <summary>
        /// Gets the max impact level.
        /// </summary>
        /// <value>The max impact level.</value>
        public byte HighestImpactLevel
        {
            get { return _highestImpactLevel; }
            private set { _highestImpactLevel = value; OnPropertyChanged("HighestImpactLevel"); }
        }

        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <value>The config.</value>
        public ConfigProperties Config
        {
            get { return _config; }
        }

        /// <summary>
        /// Gets the type of the meter.
        /// </summary>
        /// <value>The type of the meter.</value>
        public ReceiverEvents.SensorDeviceType MeterType
        {
            get { return _meterType; }
            private set { _meterType = value; OnPropertyChanged("MeterType"); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has panels.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has panels; otherwise, <c>false</c>.
        /// </value>
        public bool HasPanels
        {
            get { return _hasPanels; }
            private set { _hasPanels = value; OnPropertyChanged("HasPanels"); }
        }

        /// <summary>
        /// Gets or sets the hit panel num.
        /// </summary>
        /// <value>The hit panel num.</value>
        public int HitPanelNum
        {
            get { return _hitPanelNum; }
            set { _hitPanelNum = value; OnPropertyChanged("HitPanelNum"); }
        }

        /// <summary>
        /// Gets the color of the brush to be used for this presenter.
        /// </summary>
        /// <value>The color of the brush.</value>
        public Brush MeterColor
        {
            get { return _meterColor; }
            private set { _meterColor = value; OnPropertyChanged("MeterColor"); }
        }

        /// <summary>
        /// Gets or sets the graph data items.
        /// </summary>
        /// <value>The graph data items.</value>
        public GraphDataCollection GraphDataItems
        {
            get { return _graphDataItems; }
            private set { _graphDataItems = value; OnPropertyChanged("GraphDataItems"); }
        }

        /// <summary>
        /// Gets or sets the battery low indicator.
        /// </summary>
        /// <value>The battery low indicator.</value>
        public bool BatteryLow
        {
            get { return _batteryLow; }
            private set { _batteryLow = value; OnPropertyChanged("BatteryLow"); }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The close meter command.
        /// </summary>
        /// <value>The close meter command.</value>
        public ICommand CloseMeterCommand { get; private set; }

        /// <summary>
        /// The popup graph command.
        /// </summary>
        /// <value>The show popup graph command.</value>
        public ICommand ShowPopupGraphCommand { get; private set; }

        /// <summary>
        /// Thow popup roster command.
        /// </summary>
        /// <value>The show popup roster command.</value>
        public ICommand ShowPopupRosterCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public MeterPresenter(IUnityContainer container, IEventAggregator eventAggregator, ConfigProperties config)
        {
            _eventAggregator = eventAggregator;
            _container = container;
            _config = config;
            RegisterCommands();
            SubscribeEvents();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the type of the meter.
        /// </summary>
        /// <param name="meterType">Type of the meter.</param>
        public void SetMeterType(ReceiverEvents.SensorDeviceType meterType)
        {
            MeterType = meterType;
            DisplayName = meterType.ToString();
            HasPanels = (meterType == ReceiverEvents.SensorDeviceType.Target);
            if (meterType == ReceiverEvents.SensorDeviceType.Target)
            {
                if (_targetNumber == 0) _targetNumber = ++_targetCount;
                DisplayName += " " + _targetNumber;
            }
            MeterColor = GetBrushColorForMeter(meterType);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the brush color for meter.
        /// TODO: This should be done in XAML not here!
        /// </summary>
        /// <param name="meterType">Type of the meter.</param>
        /// <returns></returns>
        private Brush GetBrushColorForMeter(ReceiverEvents.SensorDeviceType meterType)
        {
            // Various colors to be used for targets
            Color[] targetColors = new Color[]
            {
                Color.FromRgb(0xFF, 0xBA, 0x00),
                Color.FromRgb(0xE3, 0x19, 0x39),
                Color.FromRgb(0xFF, 0x8B, 0x00),
                Color.FromRgb(0x64, 0x1C, 0xA2),
                Color.FromRgb(0x00, 0xCF, 0x60),
                Color.FromRgb(0x1E, 0x54, 0x9E),
                Color.FromRgb(0xC6, 0xD8, 0x00),
                Color.FromRgb(0xFF, 0x5F, 0x00)
            };

            switch (meterType)
            {
                case ReceiverEvents.SensorDeviceType.Chung: 
                    return new SolidColorBrush(Color.FromRgb(0x00, 0x7D, 0x0C3));

                case ReceiverEvents.SensorDeviceType.Hong:  
                    return new SolidColorBrush(Color.FromRgb(0xE3, 0x19, 0x37));

                case ReceiverEvents.SensorDeviceType.Target:
                    Color color = targetColors[(_targetNumber % targetColors.Length)];
                    return new SolidColorBrush(color);
            }

            return Brushes.Gray;
        }

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Subscribe(OnDeviceRegistered, ThreadOption.UIThread);
            _eventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Subscribe(OnDeviceStatusUpdate, ThreadOption.UIThread);
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Subscribe(OnSensorHit, ThreadOption.PublisherThread);
            _eventAggregator.GetEvent<MeterEvents.ResetMeters>().Subscribe(OnMeterReset, ThreadOption.PublisherThread);
        }

        /// <summary>
        /// Unsubscribes the events.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        private void UnsubscribeEvents()
        {
            _eventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Unsubscribe(OnDeviceRegistered);
            _eventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Unsubscribe(OnDeviceStatusUpdate);
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Unsubscribe(OnSensorHit);
            _eventAggregator.GetEvent<MeterEvents.ResetMeters>().Unsubscribe(OnMeterReset);
        }

        /// <summary>
        /// Registers the commands.
        /// </summary>
        private void RegisterCommands()
        {
            CloseMeterCommand = new DelegateCommand<object>(obj => OnRequestRemoval());

            ShowPopupGraphCommand = new DelegateCommand<object>(obj =>
            {
                _eventAggregator.GetEvent<DataGraphEvents.ShowMeterHistory>().Publish(
                    new DataGraphEvents.ShowMeterHistory() { DisplayName = DisplayName, SensorId = Id });
            });

            ShowPopupRosterCommand = new DelegateCommand<object>(obj =>
            {
                _eventAggregator.GetEvent<RosterEvents.ShowRosterPickList>().Publish(new RosterEvents.ShowRosterPickList()
                    {
                        ResultHandler = (name) => { if (name.Length > 0) DisplayName = name; }
                    });
            });
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when presenter requests to be removed by owner.
        /// </summary>
        protected override void OnRequestRemoval()
        {
            _eventAggregator.GetEvent<MeterEvents.MeterRemoved>().Publish(new MeterEvents.MeterRemoved() { SensorId = this.Id });
            UnsubscribeEvents();
            base.OnRequestRemoval();
        }

        /// <summary>
        /// Called when [meter reset].
        /// </summary>
        /// <param name="meterResetData">The meter reset data.</param>
        private void OnMeterReset(MeterEvents.ResetMetersArgs meterResetData)
        {
            switch (meterResetData)
            {
                case MeterEvents.ResetMetersArgs.All:
                    this.HighestImpactLevel = 0;
                    this.HitPanelNum = 0;
                    GraphDataItems.Clear();
                    break;

                case MeterEvents.ResetMetersArgs.High:
                    this.HighestImpactLevel = 0;
                    this.HitPanelNum = 0;
                    break;

                case MeterEvents.ResetMetersArgs.History:
                    GraphDataItems.Clear();
                    break;
            }
        }

        /// <summary>
        /// Called when [sensor registered].
        /// </summary>
        /// <param name="sensorRegistrationData">The sensor registration data.</param>
        private void OnDeviceRegistered(ReceiverEvents.DeviceRegistered e)
        {
            if (e.SensorId == this.Id)
            {
                SetMeterType(e.SensorType);
                IsActive = true;
            }
        }

        /// <summary>
        /// Called when sensor impact is received.
        /// </summary>
        /// <param name="sensorImpactData">The sensor impact data.</param>
        private void OnSensorHit(ReceiverEvents.SensorHit e)
        {
            if (e.SensorId == this.Id)
            {
                this.ImpactLevel = e.ImpactLevel;
                this.HitPanelNum = (int)e.Panel;

                if (e.ImpactLevel > this.HighestImpactLevel) 
                    this.HighestImpactLevel = e.ImpactLevel;

                // Update history
                while (_graphDataItems.Count >= 10)
                    _graphDataItems.RemoveAt(0);
                _graphDataItems.Add(e.ImpactLevel);
            }
        }

        /// <summary>
        /// Called when [sensor status update].
        /// </summary>
        /// <param name="sensorStatusData">The sensor status data.</param>
        private void OnDeviceStatusUpdate(ReceiverEvents.DeviceStatusUpdate e)
        {
            if (e.SensorId == this.Id)
            {
                switch (e.Status)
                {
                    case ReceiverEvents.SensorDeviceStatus.LowBattery:
                        if (BatteryLow == false)
                            BatteryLow = true;
                        break;

                    case ReceiverEvents.SensorDeviceStatus.NotResponding:
                        MeterColor = Brushes.Gray;
                        IsActive = false;
                        break;

                    case ReceiverEvents.SensorDeviceStatus.HoguOk:
                    case ReceiverEvents.SensorDeviceStatus.TargetOk:
                        if (MeterColor == Brushes.Gray)
                        {
                            MeterColor = GetBrushColorForMeter(MeterType);
                            IsActive = true;
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
