// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopWatchService.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Windows.Threading;

    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Stop Watch Service
    /// </summary>
    [Export(typeof(IStopWatchService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class StopWatchService : NotificationObject, IStopWatchService
    {
        #region Constants and Fields

        /// <summary>
        /// The dispatcher timer used to push updates at 1 second intervals
        /// </summary>
        protected readonly DispatcherTimer DispatchTimer = new DispatcherTimer(DispatcherPriority.Background);

        /// <summary>
        /// Composite Application Library Event Aggregator
        /// </summary>
        protected readonly IEventAggregator EventAggregator;

        /// <summary>
        /// Composite Application Library Logger
        /// </summary>
        protected readonly ILoggerFacade Logger;

        /// <summary>
        /// High Resolution Stopwatch timer used to track clock time
        /// </summary>
        protected readonly Stopwatch Stopwatch = new Stopwatch();

        /// <summary>
        /// Countdown duration (TimeSpan.Zero indicates we are counting up instead of down)
        /// </summary>
        private TimeSpan duration = TimeSpan.Zero;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StopWatchService"/> class.
        /// </summary>
        /// <param name="eventAggregator">
        /// The event aggregator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        [ImportingConstructor]
        public StopWatchService(IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this.EventAggregator = eventAggregator;
            this.Logger = logger;

            this.DispatchTimer.Interval = TimeSpan.FromMilliseconds(100);
            this.DispatchTimer.Tick += this.DispatchTimerTick;

            logger.Log("StopWatchService Initialized", Category.Debug, Priority.None);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the chronograph time to be displayed
        /// </summary>
        /// <value>The chronograph display time.</value>
        public TimeSpan DisplayTime
        {
            get
            {
                if (this.duration == TimeSpan.Zero)
                {
                    return this.Stopwatch.Elapsed;
                }
                else if (this.duration > this.Stopwatch.Elapsed)
                {
                    return this.duration - this.Stopwatch.Elapsed;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        /// <summary>
        /// Gets or sets the count down duration.
        /// TimeSpan.Zero indicates we are counting up instead of down.
        /// </summary>
        /// <value>The count down time.</value>
        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                this.duration = value;
                this.Stopwatch.Reset();
                this.RaisePropertyChanged(() => this.Duration);
                this.RaisePropertyChanged(() => this.DisplayTime);
                this.Logger.Log("StopWatchService: Countdown Duration set to " + value, Category.Debug, Priority.None);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the stopwatch is running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the stopwatch is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return this.Stopwatch.IsRunning;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IStopWatchService

        /// <summary>
        /// Pauses the count down. You can call StartCountDown to continue from this point.
        /// </summary>
        public void PauseCountDown()
        {
            this.DispatchTimer.Stop();
            this.Stopwatch.Stop();
            this.RaisePropertyChanged(() => this.IsRunning);

            this.Logger.Log("StopWatchService: Countdown paused", Category.Debug, Priority.None);
        }

        /// <summary>
        /// Resets the count down.
        /// </summary>
        public void ResetCountDown()
        {
            this.DispatchTimer.Stop();
            this.Stopwatch.Reset();
            this.RaisePropertyChanged(() => this.IsRunning);

            this.Logger.Log("StopWatchService: Countdown reset", Category.Debug, Priority.None);
        }

        /// <summary>
        /// Starts the count down.
        /// </summary>
        public void StartCountDown()
        {
            this.DispatchTimer.Start();
            this.Stopwatch.Start();
            this.RaisePropertyChanged(() => this.IsRunning);

            this.Logger.Log("StopWatchService: Countdown started", Category.Debug, Priority.None);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Tick event of the DispatchTimer control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void DispatchTimerTick(object sender, EventArgs e)
        {
            if (this.DisplayTime.TotalSeconds <= 0)
            {
                this.PauseCountDown();
            }

            this.RaisePropertyChanged(() => this.DisplayTime);
        }

        #endregion
    }
}