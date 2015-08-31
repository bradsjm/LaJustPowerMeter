// <copyright file="GameDataLoggerService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Linq;
    using System.IO;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Unity;
    using Infrastructure;
    using Graphing.Entities;

    /// <summary>
    /// Data Log Service for the PowerMeter application
    /// </summary>
    public class GameDataLoggerService : IDisposable
    {
        #region Private Members

        /// <summary>
        /// Composite Application Library Logger Reference
        /// </summary>
        private readonly ILoggerFacade Logger;

        /// <summary>
        /// Composite Application Library Event Aggregator Reference
        /// </summary>
        private readonly IEventAggregator EventAggregator;

        /// <summary>
        /// Game Data Log (In memory)
        /// </summary>
        private readonly DataLogModel GameDataLog = new DataLogModel();

        /// <summary>
        /// Data Writer Worker (dumps from memory to disk)
        /// </summary>
        private readonly System.Timers.Timer DataWriterTimer = new System.Timers.Timer(20000);

        /// <summary>
        /// Reference to the Score Keeper Service which contains scoring data
        /// </summary>
        private readonly IScoreKeeperService ScoreKeeperService;

        /// <summary>
        /// Reference to the Stop Watch Service which contains the timing data
        /// </summary>
        private readonly IStopWatchService StopWatchService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public GameDataLoggerService(
            IEventAggregator eventAggregator, 
            ILoggerFacade logger, 
            IStopWatchService stopWatchService, 
            IScoreKeeperService scoreKeeperService)
        {
            this.EventAggregator = eventAggregator;
            this.Logger = logger;
            this.ScoreKeeperService = scoreKeeperService;
            this.StopWatchService = stopWatchService;

            RegisterHandlers();
            LoadDataSet();
            if (GameDataLog.Impacts.Count > 0)
            {
                this.ScoreKeeperService.GameNumber = (byte)(this.GameDataLog.Impacts.Max(row => row.GameNumber) + 1);
            }

            DataWriterTimer.AutoReset = false;
            DataWriterTimer.Elapsed += (s, e) => WriteDatabaseToDisk();

            logger.Log("GameDataLoggerService initialized", Category.Debug, Priority.None);
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DataWriterTimer.Dispose();
                this.WriteDatabaseToDisk();
            }
        }

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        private void RegisterHandlers()
        {
            // Database events run on background threads
            this.EventAggregator.GetEvent<ReceiverEvents.SensorHit>().Subscribe(this.OnSensorHit, ThreadOption.BackgroundThread);
            this.EventAggregator.GetEvent<GameEvents.ScoreChanged>().Subscribe(this.OnScoreChanged, ThreadOption.BackgroundThread);
        }

        /// <summary>
        /// Creates the data directory.
        /// </summary>
        /// <returns></returns>
        private string CreateDataDirectory()
        {
            string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LaJust");
            Directory.CreateDirectory(dataDirectory);
            return dataDirectory;
        }

        /// <summary>
        /// Gets the name of the data set file.
        /// </summary>
        /// <returns></returns>
        private string GetDataSetFileName()
        {
            return DateTime.Today.ToString("yyyy-MM-dd") + ".dsx";
        }

        /// <summary>
        /// Loads the data set.
        /// </summary>
        private void LoadDataSet()
        {
            try
            {
                string fileName = Path.Combine(CreateDataDirectory(), GetDataSetFileName());
                if (File.Exists(fileName))
                {
                    GameDataLog.ReadXml(fileName, XmlReadMode.IgnoreSchema);
                }
            }
            catch (IOException ex)
            {
                Logger.Log(ex.GetBaseException().ToString(), Category.Exception, Priority.High);
                EventAggregator.GetEvent<ApplicationEvents.NotifyUserEvent>().Publish(new ApplicationEvents.NotifyUserEvent()
                {
                    Message = "Trying to read game data. " + ex.GetBaseException().Message,
                    Buttons = System.Windows.MessageBoxButton.OK,
                    Image = System.Windows.MessageBoxImage.Error
                });
            }
        }

        /// <summary>
        /// Writes the database to disk.
        /// </summary>
        private void WriteDatabaseToDisk()
        {
            if (GameDataLog.HasChanges())
            {
                Logger.Log("Flushing dataset to disk", Category.Info, Priority.None);
                try
                {
                    string fileName = Path.Combine(CreateDataDirectory(), GetDataSetFileName());

                    using (ReaderWriterLockMgr impactLockMgr = new ReaderWriterLockMgr(GameDataLog.Impacts.Lock))
                    using (ReaderWriterLockMgr scoreLockMgr = new ReaderWriterLockMgr(GameDataLog.Scores.Lock))
                    {
                        impactLockMgr.EnterWriteLock();
                        scoreLockMgr.EnterWriteLock();
                        foreach (var row in GameDataLog.Impacts.Where(row => row.Timestamp.Date != DateTime.Today).ToList()) GameDataLog.Impacts.RemoveImpactsRow(row);
                        foreach (var row in GameDataLog.Scores.Where(row => row.Timestamp.Date != DateTime.Today).ToList()) GameDataLog.Scores.RemoveScoresRow(row);
                        GameDataLog.AcceptChanges();
                        GameDataLog.WriteXml(fileName, XmlWriteMode.WriteSchema);
                    }
                }
                catch (IOException ex)
                {
                    Logger.Log(ex.GetBaseException().ToString(), Category.Exception, Priority.High);
                    EventAggregator.GetEvent<ApplicationEvents.NotifyUserEvent>().Publish(new ApplicationEvents.NotifyUserEvent()
                    {
                        Message = "Trying to write game data. " + ex.GetBaseException().Message,
                        Buttons = System.Windows.MessageBoxButton.OK,
                        Image = System.Windows.MessageBoxImage.Error
                    });
                }
            }
            else
            {
                Logger.Log("Skipping flushing of dataset (no changes)", Category.Info, Priority.None);
            }
        }

        /// <summary>
        /// Called when [sensor impact].
        /// </summary>
        /// <param name="sensorImpactData">The sensor impact data.</param>
        private void OnSensorHit(ReceiverEvents.SensorHit e)
        {
            if (StopWatchService.IsRunning)
            {
                using (ReaderWriterLockMgr lockMgr = new ReaderWriterLockMgr(GameDataLog.Impacts.Lock))
                {
                    lockMgr.EnterWriteLock();
                    GameDataLog.Impacts.AddImpactsRow(
                        DateTime.Now,                                       // Current datetime stamp
                        e.SensorId,                                         // Sensor ID
                        ScoreKeeperService.GameNumber,                      // Game number
                        ScoreKeeperService.RoundNumber,                     // Round number
                        StopWatchService.DisplayTime,                       // Elapsed time
                        ScoreKeeperService[e.SensorId].DisplayName,         // Competitor Name
                        e.OperationCode,                                    // Data Source Type
                        ScoreKeeperService[e.SensorId].RequiredImpactLevel, // Required impact level
                        e.ImpactLevel,                                      // Actual impact level
                        e.Panel.ToString()                                  // Sensor panel
                    );
                }
                this.DataWriterTimer.Start();
            }
        }

        /// <summary>
        /// Called when [meter score changed].
        /// </summary>
        /// <param name="meterScoreData">The meter score data.</param>
        private void OnScoreChanged(GameEvents.ScoreChanged e)
        {
            using (ReaderWriterLockMgr lockMgr = new ReaderWriterLockMgr(GameDataLog.Scores.Lock))
            {
                lockMgr.EnterWriteLock();
                GameDataLog.Scores.AddScoresRow(
                    DateTime.Now,                                           // Current datetime stamp
                    e.SensorId,                                             // Sensor ID
                    ScoreKeeperService.GameNumber,                          // Game number
                    ScoreKeeperService.RoundNumber,                         // Round number
                    StopWatchService.DisplayTime,                           // Elapsed time
                    ScoreKeeperService[e.SensorId].DisplayName,             // Meter Name
                    e.RequiredImpactLevel,                                  // Required impact level
                    e.ImpactLevel,                                          // Actual impact level
                    (int)(e.NewScore - e.OldScore),                         // Number of points
                    e.NewScore
                );
            }
        }
    }
}
