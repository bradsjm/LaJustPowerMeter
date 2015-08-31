/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Receiver.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Windows;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.EIDSS.Communications;
    using LaJust.EIDSS.Communications.Hardware;
    using LaJust.EIDSS.Communications.Entities;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;
    using System.Collections.Specialized;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class ReceiverService : PropertyNotifier, IDisposable
    {
        #region Private Properties

        private IUnityContainer _container;
        private IEventAggregator _eventAggregator;
        private IReceiverManager _receiverManager;
        private ILoggerFacade _logger;
        private ConfigProperties _config;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets the receiver count.
        /// </summary>
        /// <value>The receiver count.</value>
        public int ReceiverCount
        {
            get { return _receiverManager.Count(); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ReceiverService(IUnityContainer container, IEventAggregator aggregator, ILoggerFacade logger, ConfigProperties config)
        {
            _container = container;
            _eventAggregator = aggregator;
            _logger = logger;
            _receiverManager = new ReceiverManager(config.CourtNumber);
            _config = config;
            InitializeReceiverManager();
            SubscribeEvents();
        }
        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Initializes the receiver manager.
        /// </summary>
        private void InitializeReceiverManager()
        {
            _logger.Log("Subscribing to receiver manager events", Category.Debug, Priority.Low);
            _receiverManager.PanelButtonPressed += Receiver_PanelButtonPressed;
            _receiverManager.StrikeDetected += Receiver_StrikeDetected;
            _receiverManager.DeviceRegistered += Receiver_DeviceRegistered;
            _receiverManager.DeviceStatusUpdate += Receiver_DeviceStatusUpdate;
            _receiverManager.ReceiverCountChanged += Receiver_CountChanged;
        }

        /// <summary>
        /// Subscribes to count down events to send tones out to the receivers
        /// </summary>
        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Subscribe(button =>
            {
                if (button == RemoteEvents.Buttons.RegisterTarget)
                    foreach (var receiver in _receiverManager.GetReceivers())
                        receiver.RegisterDevice(new RegistrationSettings()
                        {
                            TouchSensorMode = _config.ContactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                            OpCode = OpCodeCmds.RegisterTarget,
                            GameNumber = 1,
                            MinimumPressure = 10
                        });

                if (button == RemoteEvents.Buttons.RegisterHong)
                    foreach (var receiver in _receiverManager.GetReceivers())
                        receiver.RegisterDevice(new RegistrationSettings()
                        {
                            TouchSensorMode = _config.ContactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                            OpCode = OpCodeCmds.RegisterHong,
                            GameNumber = 1,
                            MinimumPressure = 10
                        });

                if (button == RemoteEvents.Buttons.RegisterChung)
                    foreach (var receiver in _receiverManager.GetReceivers())
                        receiver.RegisterDevice(new RegistrationSettings()
                        {
                            TouchSensorMode = _config.ContactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                            OpCode = OpCodeCmds.RegisterChung,
                            GameNumber = 1,
                            MinimumPressure = 10
                        });

            }, ThreadOption.BackgroundThread);

            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Subscribe(state =>
            {
                foreach (Receiver recvr in _receiverManager.GetReceivers())
                {
                    switch (state)
                    {
                        case CountDownClockEvents.StateChangedArgs.Running:
                            recvr.GenerateTone(ToneTypeEnum.StartRound);
                            break;

                        case CountDownClockEvents.StateChangedArgs.Finished:
                            recvr.GenerateTone(ToneTypeEnum.EndRound);
                            break;
                    }
                }
            }, ThreadOption.BackgroundThread);
        }

        #endregion

        #region ReceiverManager Event Handlers

        /// <summary>
        /// Receivers the notification the receiver count changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LaJust.EIDSS.Communications.Manager.ReceiverCountEventArgs"/> instance containing the event data.</param>
        private void Receiver_CountChanged(object sender, ReceiverCountEventArgs e)
        {
            OnPropertyChanged("ReceiverCount");
        }

        /// <summary>
        /// Handles the PanelButtonPressed event of the Receiver control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.PanelButtonEventArgs"/> instance containing the event data.</param>
        private void Receiver_PanelButtonPressed(object sender, PanelButtonEventData e)
        {
            IReceiver receiver = e.Receiver;
            PanelButtons button = e.Button;

            // Used to keep monitor from going into suspend when we have receiver activity
            Helpers.ActivitySimulator.MoveMouse();

            _logger.Log("Publishing receiver button pushed event to EventAggregator", Category.Info, Priority.Low);
            _eventAggregator.GetEvent<ReceiverEvents.ButtonPressed>().Publish(
                    (ReceiverEvents.PanelButtons)Enum.Parse(typeof(ReceiverEvents.PanelButtons), button.ToString(), true));


            // FIXME: Move this code to the game engine module
            bool contactSensorRequired = _config.ContactSensorRequired;

            if (button == PanelButtons.ChungRegister)
            {
                receiver.RegisterDevice(new RegistrationSettings()
                {
                    TouchSensorMode = contactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                    OpCode = OpCodeCmds.RegisterChung,
                    GameNumber = 1,
                    MinimumPressure = 10
                });
            }
            else if (button == PanelButtons.HongRegister)
            {
                receiver.RegisterDevice(new RegistrationSettings()
                {
                    TouchSensorMode = contactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                    OpCode = OpCodeCmds.RegisterHong,
                    GameNumber = 1,
                    MinimumPressure = 10
                });
            }
            else if (button == PanelButtons.HongWin) // Temporary code
            {
                receiver.RegisterDevice(new RegistrationSettings()
                {
                    TouchSensorMode = contactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired,
                    OpCode = OpCodeCmds.RegisterTarget,
                    GameNumber = 1,
                    MinimumPressure = 10
                });
            }
        }

        /// <summary>
        /// Handles the DeviceRegistered event of the Receiver control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.DeviceRegisteredEventArgs"/> instance containing the event data.</param>
        private void Receiver_DeviceRegistered(object sender, DeviceRegistrationEventData e)
        {
            // Used to keep monitor from going into suspend when we have receiver activity
            Helpers.ActivitySimulator.MoveMouse();

            _logger.Log("Publishing device registration received event to EventAggregator", Category.Info, Priority.Low);
            ReceiverEvents.SensorDeviceType sensorType = ReceiverEvents.SensorDeviceType.Unknown;
            switch (e.OpCode)
            {
                case OpCodes.ChungRegistered: sensorType = ReceiverEvents.SensorDeviceType.Chung; break;
                case OpCodes.HongRegistered: sensorType = ReceiverEvents.SensorDeviceType.Hong; break;
                case OpCodes.TargetRegistered: sensorType = ReceiverEvents.SensorDeviceType.Target; break;
            }
            _eventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Publish(new ReceiverEvents.DeviceRegistered()
            { 
                SensorId = e.Id.ToString(),
                SensorType = sensorType
            });
        }

        /// <summary>
        /// Handles the StrikeDetected event of the Receiver control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.StrikeDetectedEventArgs"/> instance containing the event data.</param>
        private void Receiver_StrikeDetected(object sender, DeviceEventData e)
        {
            // Used to keep monitor from going into suspend when we have receiver activity
            Helpers.ActivitySimulator.MoveMouse();

            _logger.Log("Publishing strike received event #" + e.SequenceNumber + " to EventAggregator", Category.Info, Priority.Low);

            byte impactLevel = Math.Max(e.VestHitValue, e.HeadHitValue); 

            // FIXME: Temporary code to software patch sensor issue 12/19/09
            //if (e.WetBagPanel == WetBagPanelEnum.BottomMiddle) impactLevel = (byte)((double)impactLevel * 1.20);

            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Publish(new ReceiverEvents.SensorHit()
            {
                SensorId = e.DeviceId.ToString(),
                OpCode = e.OpCode.ToString(),
                ImpactLevel = impactLevel,
                Panel = (ReceiverEvents.SensorPanel)Enum.Parse(typeof(ReceiverEvents.SensorPanel), e.WetBagPanel.ToString(), true)
            });
        }

        /// <summary>
        /// Handles the DeviceNotResponding event of the Receiver control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LaJust.PowerMeter.Communications.DeviceNotRespondingEventArgs"/> instance containing the event data.</param>
        private void Receiver_DeviceStatusUpdate(object sender, DeviceStatusEventData e)
        {
            if (e.DeviceStatus != DeviceStatusEnum.NotResponding) Helpers.ActivitySimulator.MoveMouse();

            _logger.Log("Publishing device status update event (" + e.DeviceId.ToString() + ") to EventAggregator", Category.Info, Priority.Low);
            _eventAggregator.GetEvent<ReceiverEvents.DeviceStatusUpdate>().Publish(new ReceiverEvents.DeviceStatusUpdate()
            {
                SensorId = e.DeviceId.ToString(),
                Status = (ReceiverEvents.SensorDeviceStatus)Enum.Parse(typeof(ReceiverEvents.SensorDeviceStatus), e.DeviceStatus.ToString(), true)
            });
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_receiverManager != null)
                {
                    _receiverManager.PanelButtonPressed -= Receiver_PanelButtonPressed;
                    _receiverManager.StrikeDetected -= Receiver_StrikeDetected;
                    _receiverManager.DeviceRegistered -= Receiver_DeviceRegistered;
                    _receiverManager.DeviceStatusUpdate -= Receiver_DeviceStatusUpdate;
                    _receiverManager.ReceiverCountChanged -= Receiver_CountChanged;
                    _receiverManager.Dispose();
                }
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
