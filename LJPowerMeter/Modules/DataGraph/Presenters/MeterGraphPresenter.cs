/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.PowerMeter.Modules.DataGraph.Presenters
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Modules.DataGraph.Models;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using Microsoft.Practices.Composite.Presentation.Events;

    public class MeterGraphPresenter : Presenter
    {
        private readonly DataLogModel _dataLogModel;
        private int _gameNumber = 0;
        private EnumerableRowCollection<DataLogModel.ImpactsRow> _meterImpacts;
        private DispatcherTimer _backgroundWorker = new DispatcherTimer(DispatcherPriority.Background);
        private byte _roundNumber = 0;
        private string _sensorId;
        private string _title;
        private string _meterName;
        private string _noMeterDataText = "Loading data, please wait ...";

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return _title; }
            private set { _title = value; OnPropertyChanged("Title"); }
        }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public int GameNumber
        {
            get { return _gameNumber; }
            private set { _gameNumber = value; OnPropertyChanged("GameNumber"); }
        }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public byte RoundNumber
        {
            get { return _roundNumber; }
            private set { _roundNumber = value; OnPropertyChanged("RoundNumber"); }
        }

        /// <summary>
        /// Gets or sets the no meter data text.
        /// </summary>
        /// <value>The no meter data text.</value>
        public string NoMeterDataText
        {
            get { return _noMeterDataText; }
            set { _noMeterDataText = value; OnPropertyChanged("NoMeterDataText"); }
        }

        /// <summary>
        /// Gets or sets the meter impacts.
        /// </summary>
        /// <value>The meter impacts.</value>
        public EnumerableRowCollection<DataLogModel.ImpactsRow> MeterImpacts
        {
            get { return _meterImpacts; }
            private set { _meterImpacts = value; OnPropertyChanged("MeterImpacts"); }
        }

        #region Public Commands

        /// <summary>
        /// The close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterGraphPresenter"/> class.
        /// </summary>
        /// <param name="dataLogModel">The data log model.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public MeterGraphPresenter(DataLogModel dataLogModel)
        {
            _dataLogModel = dataLogModel;

            CloseCommand = new DelegateCommand<object>(new Action<object>(o =>
            {
                _backgroundWorker.Stop();
                OnRequestRemoval();
            }));

            _backgroundWorker.Tick += new EventHandler(BackgroundWorker_Tick);
            _backgroundWorker.Interval = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Configures the graph presenter.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="sensorId">The sensor id.</param>
        public void Configure(string meterName, string sensorId)
        {
            _meterName = meterName;
            _sensorId = sensorId;
            _backgroundWorker.Start();
        }

        /// <summary>
        /// Handles the Tick event of the BackgroundWorker dispatch timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BackgroundWorker_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            try
            {
                NoMeterDataText = "No meter data for " + _meterName;
                GameNumber = _dataLogModel.Impacts.Max(row => row.GameNumber);
                RoundNumber = _dataLogModel.Impacts.Where(row => row.GameNumber == GameNumber).Max(row => row.RoundNumber);

                var results = from impact in _dataLogModel.Impacts
                              where impact.SensorId == _sensorId &&
                                    impact.GameNumber == GameNumber &&
                                    impact.RoundNumber == RoundNumber
                              select impact;

                var scores = from score in _dataLogModel.Scores
                             where score.SensorId == _sensorId &&
                                   score.GameNumber == GameNumber &&
                                   score.RoundNumber == RoundNumber
                             select score;

                uint currentScore = 0;
                if (scores.Count() > 0) currentScore = scores.Last().NewScore;

                if (MeterImpacts == null || results.Count() != MeterImpacts.Count())
                {
                    Title = string.Format("{0} (Game {1:D3} {2}R Score {3})",
                        _meterName,
                        GameNumber,
                        RoundNumber,
                        currentScore);
                    MeterImpacts = results;
                }
            }
            catch { }
        }

    }
}
