// <copyright file="ClockViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Chronograph
{
    using System;
    using System.ComponentModel.Composition;
    using Infrastructure;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Clock Display View Model
    /// </summary>
    [Export]
    public class ClockViewModel : NotificationObject
    {
        /// <summary>
        /// The Stop Watch Service for Time Tracking
        /// </summary>
        private readonly IStopWatchService StopWatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="stopwatchService">The stopwatch service.</param>
        [ImportingConstructor]
        public ClockViewModel(IStopWatchService stopwatchService)
        {
            this.StopWatchService = stopwatchService;
            this.StopWatchService.OnChanged(o => o.DisplayTime).Do(delegate 
            {
                RaisePropertyChanged(() => this.ClockTime); 
            });
            this.StopWatchService.OnChanged(o => o.IsRunning).Do(delegate 
            {
                RaisePropertyChanged(() => this.IsRunning);
                RaisePropertyChanged(() => this.IsEditable);
            });
        }

        /// <summary>
        /// Gets or sets the clock time. If counting down, returns the remaining time left.
        /// </summary>
        /// <value>The clock time.</value>
        public TimeSpan ClockTime
        {
            get { return this.StopWatchService.DisplayTime; }
            set { this.StopWatchService.Duration = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is editable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditable
        {
            get 
            { 
                return 
                    this.StopWatchService.IsRunning == false && 
                    this.ClockTime != TimeSpan.Zero && 
                    this.StopWatchService.Duration != TimeSpan.Zero; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return this.StopWatchService.IsRunning; }
        }
    }
}
