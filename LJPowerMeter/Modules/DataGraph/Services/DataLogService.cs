/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.DataGraph.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Linq;
    using System.IO;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Common.Models;
    using LaJust.PowerMeter.Modules.DataGraph.Helpers;
    using LaJust.PowerMeter.Modules.DataGraph.Models;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Data Log Service for the PowerMeter application
    /// </summary>
    public class DataLogService
    {
        #region Private Members
        
        private readonly DataLogModel _dataLog;
        private readonly IUnityContainer _container;
        private readonly ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly Dictionary<string, string> _meterNames = new Dictionary<string, string>();

        private DateTime _countdownStartedTime;
        private bool _clockRunning;
        private GameMetaDataModel _game;
        private ConfigProperties _config;

        private BackgroundWorker _databaseWriter = new BackgroundWorker();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public DataLogService(IUnityContainer container, IEventAggregator eventAggregator, ILoggerFacade logger, 
            ConfigProperties config, DataLogModel dataLog, GameMetaDataModel game)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dataLog = dataLog;
            _game = game;
            _config = config;
            SubscribeEvents();
            LoadDataSet();
            if (dataLog.Impacts.Count > 0)
            {
                game.GameNumber = (byte)(dataLog.Impacts.Max(row => row.GameNumber) + 1);
            }
            _databaseWriter.DoWork += (sender, e) => WriteDatabaseToDisk();
        }

        #endregion

        /// <summary>
        /// Subscribes the events.
        /// </summary>
        private void SubscribeEvents()
        {
            _eventAggregator.GetEvent<ProcessEvent>().Subscribe(OnProcessEvent);
            _eventAggregator.GetEvent<CountDownClockEvents.StateChanged>().Subscribe(OnCountStateChanged);
            _eventAggregator.GetEvent<OnMeterDisplayNameChangedEvent>().Subscribe(OnMeterDisplayNameChanged);

            // Database events run on background threads
            _eventAggregator.GetEvent<ReceiverEvents.SensorHit>().Subscribe(OnSensorHit, ThreadOption.BackgroundThread);
            _eventAggregator.GetEvent<GameEngineEvents.ScoreChanged>().Subscribe(OnScoreChanged, ThreadOption.BackgroundThread);
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
                    _dataLog.ReadXml(fileName, XmlReadMode.IgnoreSchema);
                }
            }
            catch (IOException ex)
            {
                _logger.Log(ex.GetBaseException().ToString(), Category.Exception, Priority.High);
                //FIXME: Need to handle this exception with a user notification
            }
        }

        #region Event Subscription Handlers

        /// <summary>
        /// Called when [process event].
        /// </summary>
        /// <param name="processEvent">The process event.</param>
        private void OnProcessEvent(ProcessEventType processEvent)
        {
            if (processEvent == ProcessEventType.ApplicationShutdown && !_databaseWriter.IsBusy) 
                WriteDatabaseToDisk();
        }

        /// <summary>
        /// Writes the database to disk.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void WriteDatabaseToDisk()
        {
            if (_dataLog.HasChanges())
            {
                _logger.Log("Flushing dataset to disk", Category.Info, Priority.None);
                try
                {
                    string fileName = Path.Combine(CreateDataDirectory(), GetDataSetFileName());

                    using (ReaderWriterLockMgr impactLockMgr = new ReaderWriterLockMgr(_dataLog.Impacts.Lock))
                    using (ReaderWriterLockMgr scoreLockMgr = new ReaderWriterLockMgr(_dataLog.Scores.Lock))
                    {
                        impactLockMgr.EnterWriteLock();
                        scoreLockMgr.EnterWriteLock();
                        foreach (var row in _dataLog.Impacts.Where(row => row.Timestamp.Date != DateTime.Today).ToList()) _dataLog.Impacts.RemoveImpactsRow(row);
                        foreach (var row in _dataLog.Scores.Where(row => row.Timestamp.Date != DateTime.Today).ToList()) _dataLog.Scores.RemoveScoresRow(row);
                        _dataLog.AcceptChanges();
                        _dataLog.WriteXml(fileName, XmlWriteMode.WriteSchema);
                    }
                }
                catch (IOException ex)
                {
                    _logger.Log(ex.GetBaseException().ToString(), Category.Exception, Priority.High);
                    //FIXME: Need to handle this exception with a user notification
                }
            }
            else
            {
                _logger.Log("Skipping flushing of dataset (no changes)", Category.Info, Priority.None);
            }
        }

        /// <summary>
        /// Called when [count state changed].
        /// </summary>
        /// <param name="countDownState">State of the count down.</param>
        private void OnCountStateChanged(CountDownClockEvents.StateChangedArgs countDownState)
        {
            switch (countDownState)
            {
                case CountDownClockEvents.StateChangedArgs.Running:
                    _countdownStartedTime = DateTime.Now;
                    _clockRunning = true;
                    break;

                case CountDownClockEvents.StateChangedArgs.Reset:
                case CountDownClockEvents.StateChangedArgs.Paused:
                    _clockRunning = false;
                    break;

                case CountDownClockEvents.StateChangedArgs.Finished:
                    _clockRunning = false;
                    if (!_databaseWriter.IsBusy) _databaseWriter.RunWorkerAsync();
                    break;
            }
        }

        /// <summary>
        /// Called when [meter display name changed].
        /// </summary>
        /// <param name="meterNameChangeData">The meter name change data.</param>
        private void OnMeterDisplayNameChanged(MeterDisplayNameChangedArgs meterNameChangeData)
        {
            if (_meterNames.ContainsKey(meterNameChangeData.SensorId))
                _meterNames[meterNameChangeData.SensorId] = meterNameChangeData.NewMeterName;
            else
                _meterNames.Add(meterNameChangeData.SensorId, meterNameChangeData.NewMeterName);
        }

        /// <summary>
        /// Called when [sensor impact].
        /// </summary>
        /// <param name="sensorImpactData">The sensor impact data.</param>
        private void OnSensorHit(ReceiverEvents.SensorHit e)
        {
            if (_clockRunning)
            {
                using (ReaderWriterLockMgr lockMgr = new ReaderWriterLockMgr(_dataLog.Impacts.Lock))
                {
                    lockMgr.EnterWriteLock();
                    _dataLog.Impacts.AddImpactsRow(
                        DateTime.Now,                           // Current datetime stamp
                        e.SensorId,                             // Sensor ID
                        _game.GameNumber,                       // Game number
                        _game.RoundNumber,                      // Round number
                        DateTime.Now - _countdownStartedTime,   // Elapsed time
                        (_meterNames.ContainsKey(e.SensorId) ? _meterNames[e.SensorId] : string.Empty), // Meter Name
                        e.OpCode,                               // Data Source Type
                        _config.RequiredImpactLevel,             // required impact level
                        e.ImpactLevel,                          // Actual impact level
                        e.Panel.ToString()                      // Sensor panel
                    );
                }
            }
        }

        /// <summary>
        /// Called when [meter score changed].
        /// </summary>
        /// <param name="meterScoreData">The meter score data.</param>
        private void OnScoreChanged(GameEngineEvents.ScoreChanged e)
        {
            using (ReaderWriterLockMgr lockMgr = new ReaderWriterLockMgr(_dataLog.Scores.Lock))
            {
                lockMgr.EnterWriteLock();
                _dataLog.Scores.AddScoresRow(
                    DateTime.Now,                           // Current datetime stamp
                    e.SensorId,                             // Sensor ID
                    _game.GameNumber,                       // Game number
                    _game.RoundNumber,                      // Round number
                    DateTime.Now - _countdownStartedTime,   // Elapsed time
                    (_meterNames.ContainsKey(e.SensorId) ? _meterNames[e.SensorId] : string.Empty), // Meter Name
                    _config.RequiredImpactLevel,             // required impact level
                    e.ImpactLevel,                          // Actual impact level
                    (int)(e.NewScore - e.OldScore),         // Number of points
                    e.NewScore
                );
            }
        }

        #endregion

    }
}
