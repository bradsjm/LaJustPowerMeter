/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.CountDownClock.Presenters
{
    using System;
    using System.Diagnostics;
    using System.Windows.Media;
    using System.Windows.Threading;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Commands;

    public class CountDownClockPresenter : Presenter
    {
        #region Private Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly ConfigProperties _config;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Background);

        private TimeSpan _countDown = TimeSpan.Zero;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the clock time. If counting down, returns the remaining time left.
        /// </summary>
        /// <value>The clock time.</value>
        public TimeSpan ClockTime
        {
            get
            {
                if (_countDown == TimeSpan.Zero)
                    return _stopWatch.Elapsed;
                else if (_countDown > _stopWatch.Elapsed)
                    return (_countDown - _stopWatch.Elapsed);
                else
                    return TimeSpan.Zero;
            }
            set
            {
                _countDown = value;
                _stopWatch.Reset();
                OnPropertyChanged("ClockTime");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is editable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditable
        {
            get { return _stopWatch.IsRunning == false && ClockTime != TimeSpan.Zero && _countDown != TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public Brush BackgroundColor
        {
            get { return _stopWatch.IsRunning ? Brushes.Yellow : Brushes.White; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockPresenter"/> class.
        /// </summary>
        public CountDownClockPresenter(IEventAggregator eventAggregator, ConfigProperties config)
        {
            _eventAggregator = eventAggregator;
            _config = config;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;
            eventAggregator.GetEvent<CountDownClockEvents.ChangeState>().Subscribe(OnCountDownStateChanged);
            ResetCountDown();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [count down state changed].
        /// </summary>
        /// <param name="countDownState">State of the count down.</param>
        private void OnCountDownStateChanged(CountDownClockEvents.ChangeStateArgs countDownState)
        {
            switch (countDownState)
            {
                case CountDownClockEvents.ChangeStateArgs.Start: StartCountDown(); break;
                case CountDownClockEvents.ChangeStateArgs.Pause: PauseCountDown(); break;
                case CountDownClockEvents.ChangeStateArgs.Reset: ResetCountDown(); break;
            }
        }

        /// <summary>
        /// Handles the Tick event of the Timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ClockTime.TotalSeconds <= 0)
            {
                PauseCountDown();
                _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Publish(CountDownClockEvents.StateChangedArgs.Finished);
            }
            OnPropertyChanged("ClockTime");
        }

        #endregion

        #region Private Method Implementation

        /// <summary>
        /// Starts the count down.
        /// </summary>
        private void StartCountDown()
        {
            // Enable the one second interval timer
            _timer.Start();

            // Start the stop watch
            _stopWatch.Start();

            // Update the clock background color
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("IsEditable");

            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Publish(CountDownClockEvents.StateChangedArgs.Running);
        }

        /// <summary>
        /// Pauses the count down.
        /// </summary>
        private void PauseCountDown()
        {
            // Stop the stopwatch
            _stopWatch.Stop();

            // Stop the one second interval timer
            _timer.Stop();

            // Update the clock background color
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("IsEditable");

            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Publish(CountDownClockEvents.StateChangedArgs.Paused);
        }

        /// <summary>
        /// Resets the count down.
        /// </summary>
        /// <param name="param">The param.</param>
        private void ResetCountDown()
        {
            // Reset the stop watch to zero and notify clocktime has changed
            _timer.Stop();
            _stopWatch.Reset();
            _countDown = _config.RoundDuration;
            OnPropertyChanged("ClockTime");

            // Update the clock background color
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("IsEditable");

            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Publish(CountDownClockEvents.StateChangedArgs.Reset);
        }

        #endregion

    }
}
