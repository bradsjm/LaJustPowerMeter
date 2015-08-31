/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.GameEngine.FitnessTest
{
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Common.Models;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Receiver Service for the PowerMeter application
    /// </summary>
    public class FitnessTestGameService : IGameService
    {
        #region Private Fields

        private IUnityContainer _container;
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private ConfigProperties _config;
        private GameMetaDataModel _game;

        private CountDownClockEvents.StateChangedArgs _clockState = CountDownClockEvents.StateChangedArgs.Reset;
        private IRegionViewRegistry _regionRegistry;
        private DelegateCommand<string> _startCommand;
        private DelegateCommand<string> _pauseCommand;
        private DelegateCommand<string> _resetCommand;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public FitnessTestGameService(IUnityContainer container, ILoggerFacade logger, ConfigProperties config, IRegionViewRegistry regionRegistry, IEventAggregator eventAggregator, GameMetaDataModel game)
        {
            _container = container;
            _game = game;
            _logger = logger;
            _config = config;
            _eventAggregator = eventAggregator;
            _regionRegistry = regionRegistry;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            _logger.Log("FitnessTestGameService started", Category.Info, Priority.Low);
            SubscribeEvents();
            RegisterToolbarButtons();
        }

        /// <summary>
        /// Registers the toolbar buttons.
        /// </summary>
        private void RegisterToolbarButtons()
        {
            _startCommand = new DelegateCommand<string>(
                s => GameRoundStart(), s => _clockState != CountDownClockEvents.StateChangedArgs.Running);

            _pauseCommand = new DelegateCommand<string>(s => GameRoundPause(), 
                s => _clockState == CountDownClockEvents.StateChangedArgs.Running);

            _resetCommand = new DelegateCommand<string>( s => GameRoundReset() );

            // Register the menu buttons
            _regionRegistry.RegisterViewWithRegion(RegionNames.ToolbarLeftRegion, () =>
            {
                return new ToolBarItemModel()
                {
                    Text = "Start",
                    Icon = "/LaJust.PowerMeter.Modules.GameEngine;component/Resources/Start.png",
                    Command = _startCommand
                };
            });

            // Register the menu buttons
            _regionRegistry.RegisterViewWithRegion(RegionNames.ToolbarLeftRegion, () =>
            {
                return new ToolBarItemModel()
                {
                    Text = "Pause",
                    Icon = "/LaJust.PowerMeter.Modules.GameEngine;component/Resources/Stop.png",
                    Command = _pauseCommand
                };
            });

            _regionRegistry.RegisterViewWithRegion(RegionNames.ToolbarLeftRegion, () =>
            {
                return new ToolBarItemModel()
                {
                    Text = "Reset",
                    Icon = "/LaJust.PowerMeter.Modules.GameEngine;component/Resources/Reset.png",
                    Command = _resetCommand
                };
            });
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _logger.Log("FitnessTestGameService stopped", Category.Info, Priority.Low);
            UnsubscribeEvents();
        }

        #endregion

        #region Event Subscription Methods

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        private void SubscribeEvents()
        {
            // Receiver and Remote Button Events
            _eventAggregator.GetEvent<ReceiverEvents.ButtonPressed>().Subscribe(OnReceiverButtonPress);
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Subscribe(OnRemoteButtonPress);

            // Countdown ended
            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Subscribe(OnCountDownStateChanged);

            // Sensor Events
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Subscribe(OnSensorHit);
        }

        /// <summary>
        /// Unsubscribes the events.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        private void UnsubscribeEvents()
        {
            // Receiver Events
            _eventAggregator.GetEvent<ReceiverEvents.ButtonPressed>().Unsubscribe(OnReceiverButtonPress);
            _eventAggregator.GetEvent<RemoteEvents.ButtonPressed>().Unsubscribe(OnRemoteButtonPress);

            // Countdown changed state
            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Unsubscribe(OnCountDownStateChanged);

            // Sensor Events
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Unsubscribe(OnSensorHit);
        }

        #endregion

        #region Game Round Methods

        /// <summary>
        /// Starts the game round. If the clock is paused, then resumes the existing round.
        /// </summary>
        private void GameRoundStart()
        {
            if (_clockState == CountDownClockEvents.StateChangedArgs.Paused)
            {
                _eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Publish(CountDownClockEvents.ChangeStateArgs.Start);
            }
            else if (_clockState != CountDownClockEvents.StateChangedArgs.Running)
            {
                _eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Publish(CountDownClockEvents.ChangeStateArgs.Reset);
                _eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Publish(CountDownClockEvents.ChangeStateArgs.Start);
            }
        }

        /// <summary>
        /// Pauses an existing round
        /// </summary>
        private void GameRoundPause()
        {
            if (_clockState == CountDownClockEvents.StateChangedArgs.Running)
                _eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Publish(CountDownClockEvents.ChangeStateArgs.Pause);
        }

        /// <summary>
        /// Resets the game round by clearing the current scores and resetting the clock counter
        /// </summary>
        private void GameRoundReset()
        {
            _eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Publish(CountDownClockEvents.ChangeStateArgs.Reset);
            _eventAggregator.GetEvent<MeterEvents.ResetMeters>().Publish(MeterEvents.ResetMetersArgs.All);
            ClearCurrentScores();
            //TODO: Remove existing scores from database
        }

        /// <summary>
        /// Moves the game to the next round or the next game if all rounds used when the countdown finishes
        /// </summary>
        private void GameNextRound()
        {
            if (_game.RoundNumber >= _config.RoundsPerGame)
            {
                _game.GameNumber++;
                _game.RoundNumber = 1;
            }
            else
            {
                _game.RoundNumber++;
            }
        }

        /// <summary>
        /// Clears the current scores for all meters.
        /// </summary>
        private void ClearCurrentScores()
        {
            foreach (ScoreModel scoreModel in _container.ResolveAll<ScoreModel>())
                scoreModel.Reset();
        }

        #endregion

        #region Receiver Event Handlers

        /// <summary>
        /// Called when [receiver button press].
        /// </summary>
        /// <param name="receiverButtonPressData">The receiver button press data.</param>
        private void OnReceiverButtonPress(ReceiverEvents.PanelButtons e)
        {
            switch (e)
            {
                case ReceiverEvents.PanelButtons.Start:
                    GameRoundStart();
                    break;
                case ReceiverEvents.PanelButtons.Clocking:
                    if (_clockState == CountDownClockEvents.StateChangedArgs.Running)
                        GameRoundPause();
                    else
                        GameRoundReset();
                    break;
            }

            //TODO: Move code for registrations here from the receiver module
        }

        /// <summary>
        /// Called when [remote button press].
        /// </summary>
        /// <param name="remoteControlButton">The remote control button.</param>
        private void OnRemoteButtonPress(RemoteEvents.Buttons button)
        {
            switch (button)
            {
                case RemoteEvents.Buttons.Start:
                    GameRoundStart();
                    break;

                case RemoteEvents.Buttons.Stop:
                    if (_clockState == CountDownClockEvents.StateChangedArgs.Running)
                        GameRoundPause();
                    else
                        GameRoundReset();
                    break;

                case RemoteEvents.Buttons.Left:
                    _config.RequiredImpactLevel -= 5;
                    break;

                case RemoteEvents.Buttons.Right:
                    _config.RequiredImpactLevel += 5;
                    break;
            }
        }

        #endregion

        #region Sensor Event Handlers

        /// <summary>
        /// Called when [sensor impact].
        /// </summary>
        /// <param name="sensorImpactData">The sensor impact data.</param>
        private void OnSensorHit(ReceiverEvents.SensorHit e)
        {
            if (_clockState == CountDownClockEvents.StateChangedArgs.Running)
            {
                if (e.ImpactLevel >= _config.RequiredImpactLevel)
                {
                    ScoreModel scoreModel = _container.Resolve<ScoreModel>(e.SensorId);
                    uint oldScore = scoreModel.Value;
                    scoreModel.Value++;
                    _logger.Log(string.Format("Meter Id {0} score now {1}", e.SensorId, scoreModel.Value), Category.Info, Priority.None);
                    _eventAggregator.GetEvent<GameEngineEvents.ScoreChanged>().Publish(
                        new GameEngineEvents.ScoreChanged() { SensorId = e.SensorId, ImpactLevel = e.ImpactLevel, OldScore = oldScore, NewScore = scoreModel.Value });
                }
            }
        }

        #endregion

        #region Clock State Event Handler

        /// <summary>
        /// Called when [count down state changed].
        /// </summary>
        /// <param name="countDownState">State of the count down.</param>
        private void OnCountDownStateChanged(CountDownClockEvents.StateChangedArgs countDownState)
        {
            _clockState = countDownState;
            _startCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _resetCommand.RaiseCanExecuteChanged();

            if (countDownState == CountDownClockEvents.StateChangedArgs.Finished) GameNextRound();
        }

        #endregion
    }
}