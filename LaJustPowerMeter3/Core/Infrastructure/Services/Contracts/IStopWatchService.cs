// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStopWatchService.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Stop Watch Service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Stop Watch Service
    /// </summary>
    public interface IStopWatchService : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the display time.
        /// </summary>
        /// <value>The display time.</value>
        TimeSpan DisplayTime { get; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets a value indicating whether the stopwatch is running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the stopwatch is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses the count down.
        /// </summary>
        void PauseCountDown();

        /// <summary>
        /// Resets the count down.
        /// </summary>
        void ResetCountDown();

        /// <summary>
        /// Starts the count down.
        /// </summary>
        void StartCountDown();

        #endregion
    }
}