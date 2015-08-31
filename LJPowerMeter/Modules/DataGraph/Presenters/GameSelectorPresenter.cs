/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */

namespace LaJust.PowerMeter.Modules.DataGraph.Presenters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Modules.DataGraph.Models;
    using LaJust.PowerMeter.Modules.DataGraph.Services;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Published when the select game data changes to allow the graph to be displayed
    /// </summary>
    public class SelectedDataChangedEvent : CompositePresentationEvent<SelectedDataChangedEvent>
    {
        public EnumerableRowCollection<DataLogModel.ImpactsRow> ImpactData { get; set; }
        public EnumerableRowCollection<DataLogModel.ScoresRow> ScoreData { get; set; }
    }

    public class GameSelectorPresenter : Presenter
    {
        #region Private Fields

        private readonly IUnityContainer _container;
        private readonly GameSelectorService _service;
        private readonly IEventAggregator _eventAggregator;
        private ReadOnlyCollection<DateTime> _availableDates;
        private DataLogModel _gameData;

        #endregion

        #region Private Property Backing Fields

        private DateTime _firstAvailableDate;
        private DateTime _gameDate;
        private byte _firstGameNumber;
        private byte _lastGameNumber;
        private byte _gameNumber;
        private byte _roundNumber;
        private byte _lastRoundNumber;
        private ObservableCollection<string> _competitorNames = new ObservableCollection<string>();
        private string _competitor;
        private bool _isDataAvailable;

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the first available date.
        /// </summary>
        /// <value>The first available date.</value>
        public DateTime FirstAvailableDate
        {
            get { return _firstAvailableDate; }
            set { _firstAvailableDate = value; OnPropertyChanged("FirstAvailableDate"); }
        }

        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>The selected date.</value>
        public DateTime GameDate
        {
            get { return _gameDate; }
            set { _gameDate = value; OnPropertyChanged("GameDate"); }
        }

        /// <summary>
        /// Gets or sets the first available game number.
        /// </summary>
        /// <value>The first available game number.</value>
        public byte FirstGameNumber
        {
            get { return _firstGameNumber; }
            set { _firstGameNumber = value; OnPropertyChanged("FirstGameNumber"); }
        }

        /// <summary>
        /// Gets or sets the last game number.
        /// </summary>
        /// <value>The last game number.</value>
        public byte LastGameNumber
        {
            get { return _lastGameNumber; }
            set { _lastGameNumber = value; OnPropertyChanged("LastGameNumber"); }
        }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber
        {
            get { return _gameNumber; }
            set { _gameNumber = value; OnPropertyChanged("GameNumber"); }
        }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public byte RoundNumber
        {
            get { return _roundNumber; }
            set { _roundNumber = value; OnPropertyChanged("RoundNumber"); }
        }

        /// <summary>
        /// Gets or sets the last round number.
        /// </summary>
        /// <value>The last round number.</value>
        public byte LastRoundNumber
        {
            get { return _lastRoundNumber; }
            set { _lastRoundNumber = value; OnPropertyChanged("LastRoundNumber"); }
        }

        /// <summary>
        /// Gets or sets the competitor names.
        /// </summary>
        /// <value>The competitor names.</value>
        public ObservableCollection<string> CompetitorNames
        {
            get { return _competitorNames; }
            set { _competitorNames = value; OnPropertyChanged("CompetitorNames"); }
        }

        /// <summary>
        /// Gets or sets the competitor.
        /// </summary>
        /// <value>The competitor.</value>
        public string Competitor
        {
            get { return _competitor; }
            set { _competitor = value; OnPropertyChanged("Competitor"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether data is available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has data available; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataAvailable
        {
            get { return _isDataAvailable; }
            set { _isDataAvailable = value; OnPropertyChanged("IsDataAvailable"); }
        }

        #endregion

        #region Public Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSelectorPresenter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public GameSelectorPresenter(IUnityContainer container, IEventAggregator eventAggregator, GameSelectorService service)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _service = service;
            _eventAggregator.GetEvent<ScreenEvents.ActiveScreenChanged>().Subscribe(OnActiveScreenChanged, ThreadOption.BackgroundThread);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Called when [active screen changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnActiveScreenChanged(ScreenEvents.ActiveScreenChanged e)
        {
            //TODO: The presenter shouldn't hard code the page here (should be agnostic), find a better way?
            if (e.PageName == LaJust.PowerMeter.Common.PageNames.HistoryPage)
            {
                if (_availableDates == null) UpdateCalendar();
                if (this.GameDate == DateTime.Today) UpdateGameDetails();
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (propertyName == "GameDate") UpdateGameDetails();
                else if (propertyName == "GameNumber") UpdateRoundDetails();
                else if (propertyName == "RoundNumber") UpdateCompetitorList();
                else if (propertyName == "Competitor") UpdateCompetitorGraph();
            });
        }

        /// <summary>
        /// Updates the calendar.
        /// </summary>
        private void UpdateCalendar()
        {
            _availableDates = _service.GetAvailableDates();
            if (_availableDates.Count > 0)
                this.FirstAvailableDate = _availableDates.Min();
            this.IsDataAvailable = false;
            this.GameDate = DateTime.Today;
        }

        /// <summary>
        /// Updates the game details.
        /// </summary>
        private void UpdateGameDetails()
        {
            this.IsDataAvailable = false;

            _gameData = _service.GetDataSet(this.GameDate);

            if (_gameData != null && _gameData.Impacts.Count > 0)
            {
                this.IsDataAvailable = true;
                this.FirstGameNumber = _gameData.Impacts.Min(row => row.GameNumber);
                this.LastGameNumber = _gameData.Impacts.Max(row => row.GameNumber);
                this.GameNumber = this.FirstGameNumber;
            }
        }

        /// <summary>
        /// Updates the round details.
        /// </summary>
        private void UpdateRoundDetails()
        {
            if (_gameData != null)
            {
                var gameImpacts = _gameData.Impacts.Where(row => row.GameNumber == this.GameNumber);
                if (gameImpacts.Count() > 0)
                {
                    this.LastRoundNumber = gameImpacts.Max(row => row.RoundNumber);
                    this.RoundNumber = gameImpacts.Min(row => row.RoundNumber);
                }
                else
                {
                    this.LastRoundNumber = 1;
                    this.RoundNumber = 1;
                }
            };
        }

        /// <summary>
        /// Updates the competitor list.
        /// </summary>
        private void UpdateCompetitorList()
        {
            this.CompetitorNames = null;

            if (_gameData != null)
            {
                var names = from impact in _gameData.Impacts
                            where impact.GameNumber == this.GameNumber &&
                                  impact.RoundNumber == this.RoundNumber
                            orderby impact.Name ascending
                            group impact by impact.Name
                                into name
                                select name.Key;

                if (names.Count() > 0)
                {
                    this.CompetitorNames = new ObservableCollection<string>(names);
                    if (names.Contains(this.Competitor))
                        OnPropertyChanged("Competitor");
                    else
                        this.Competitor = names.First();
                }
            };
        }

        /// <summary>
        /// Updates the competitor graph.
        /// </summary>
        private void UpdateCompetitorGraph()
        {
            if (_gameData != null)
            {
                var impacts = from impact in _gameData.Impacts
                              where impact.GameNumber == this.GameNumber &&
                                    impact.RoundNumber == this.RoundNumber &&
                                    impact.Name == this.Competitor
                              select impact;

                var scores = from score in _gameData.Scores
                             where score.GameNumber == this.GameNumber &&
                                    score.RoundNumber == this.RoundNumber &&
                                    score.Name == this.Competitor
                             select score;

                _eventAggregator.GetEvent<SelectedDataChangedEvent>().Publish(new SelectedDataChangedEvent() 
                    {
                        ImpactData = impacts,
                        ScoreData = scores
                    }
                );
            }
        }

        #endregion

    }
}
