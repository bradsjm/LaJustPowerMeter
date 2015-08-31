// <copyright file="GameHistoryViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using Infrastructure;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Unity;
    using Graphing.Entities;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Game History View Model
    /// </summary>
    public class GameHistoryViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// Game History Services Reference for loading game data from history files
        /// </summary>
        private readonly GameHistoryService GameHistoryService;

        /// <summary>
        /// Reference for the current game data loaded from the history service
        /// </summary>
        private DataLogModel gameData;

        /// <summary>
        /// The currently selected impact data
        /// </summary>
        private EnumerableRowCollection<DataLogModel.ImpactsRow> impactData;

        /// <summary>
        /// The list of competitors in the currently selected game and round
        /// </summary>
        private DispatchingObservableCollection<string> competitorNames;

        /// <summary>
        /// The currently selected competitor
        /// </summary>
        private string selectedCompetitor;
        private string title;

        private DateTime _firstAvailableDate;
        private DateTime _gameDate;
        private byte _firstGameNumber;
        private byte _lastGameNumber;
        private byte _gameNumber;
        private byte _roundNumber;
        private byte _lastRoundNumber;
        private bool _isDataAvailable;
        private DelegateCommand<object> _printCommand;
        private DelegateCommand<object> _exportCommand;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSelectorPresenter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public GameHistoryViewModel(IEventAggregator eventAggregator, ILoggerFacade logger) : base(eventAggregator, logger)
        {
            this.GameHistoryService = new GameHistoryService();
            this.RegisterHandlers();
        }

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets the print command.
        /// </summary>
        /// <value>The print command.</value>
        public DelegateCommand<object> PrintCommand
        {
            get { return _printCommand; }
        }

        /// <summary>
        /// Gets the export command.
        /// </summary>
        /// <value>The export command.</value>
        public DelegateCommand<object> ExportCommand
        {
            get { return _exportCommand; }
        }

        /// <summary>
        /// Gets the title of the graph.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return this.title; }
            private set { this.title = value; RaisePropertyChanged("Title"); }
        }

        /// <summary>
        /// Gets the game data.
        /// </summary>
        /// <value>The game data.</value>
        public EnumerableRowCollection<DataLogModel.ImpactsRow> ImpactData
        {
            get { return this.impactData; }
            private set { this.impactData = value; RaisePropertyChanged("ImpactData"); }
        }

        /// <summary>
        /// Gets the first available date with data.
        /// </summary>
        /// <value>The first available date.</value>
        public DateTime FirstAvailableGameDate
        {
            get { return _firstAvailableDate; }
            private set { _firstAvailableDate = value; RaisePropertyChanged("FirstAvailableGameDate"); }
        }

        /// <summary>
        /// Gets or sets the selected game date.
        /// </summary>
        /// <value>The selected game date.</value>
        public DateTime SelectedGameDate
        {
            get { return _gameDate; }
            set { _gameDate = value; RaisePropertyChanged("SelectedGameDate"); }
        }

        /// <summary>
        /// Gets the first available game number.
        /// </summary>
        /// <value>The first available game number.</value>
        public byte FirstAvailableGameNumber
        {
            get { return _firstGameNumber; }
            private set { _firstGameNumber = value; RaisePropertyChanged("FirstAvailableGameNumber"); }
        }

        /// <summary>
        /// Gets the last game number.
        /// </summary>
        /// <value>The last game number.</value>
        public byte LastAvailableGameNumber
        {
            get { return _lastGameNumber; }
            private set { _lastGameNumber = value; RaisePropertyChanged("LastAvailableGameNumber"); }
        }

        /// <summary>
        /// Gets the last round number.
        /// </summary>
        /// <value>The last round number.</value>
        public byte LastAvailableRoundNumber
        {
            get { return _lastRoundNumber; }
            private set { _lastRoundNumber = value; RaisePropertyChanged("LastAvailableRoundNumber"); }
        }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte SelectedGameNumber
        {
            get { return _gameNumber; }
            set { _gameNumber = value; RaisePropertyChanged("SelectedGameNumber"); }
        }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public byte SelectedRoundNumber
        {
            get { return _roundNumber; }
            set { _roundNumber = value; RaisePropertyChanged("SelectedRoundNumber"); }
        }

        /// <summary>
        /// Gets or sets the competitor names.
        /// </summary>
        /// <value>The competitor names.</value>
        public DispatchingObservableCollection<string> CompetitorNames
        {
            get { return this.competitorNames; }
            private set { this.competitorNames = value; RaisePropertyChanged("CompetitorNames"); }
        }

        /// <summary>
        /// Gets or sets the selected competitor.
        /// </summary>
        /// <value>The competitor.</value>
        public string SelectedCompetitor
        {
            get { return this.selectedCompetitor; }
            set { this.selectedCompetitor = value; RaisePropertyChanged("SelectedCompetitor"); }
        }

        /// <summary>
        /// Gets the value indicating whether data is available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has data available; otherwise, <c>false</c>.
        /// </value>
        public bool IsDataAvailable
        {
            get { return _isDataAvailable; }
            private set { _isDataAvailable = value; RaisePropertyChanged("IsDataAvailable"); }
        }

        #endregion

        #region Private Helper Methods

        protected void RegisterHandlers()
        {
            _printCommand = new DelegateCommand<object>(o => PrintGraph());
            _exportCommand = new DelegateCommand<object>(o => ExportData());
        }

        /// <summary>
        /// Prints the graph.
        /// </summary>
        protected void PrintGraph()
        {
            GameImpactPrintView printView = new GameImpactPrintView();
            printView.ApplyViewModel(this);
            printView.Print();
        }

        /// <summary>
        /// Exports the graph data.
        /// </summary>
        protected void ExportData()
        {
            using (new ShowBusyIndicator())
            {
                ExcelExportService excelService = new ExcelExportService();
                excelService.Export(this.ImpactData);
            }
            EventAggregator.GetEvent<ApplicationEvents.NotifyUserEvent>().Publish(new ApplicationEvents.NotifyUserEvent()
            {
                Message = "Graph data has been exported to Excel file in \"My Documents\".",
                Image = System.Windows.MessageBoxImage.Information,
                Buttons = System.Windows.MessageBoxButton.OK
            });
        }

        /// <summary>
        /// Called when [active screen changed].
        /// </summary>
        /// <param name="e">The e.</param>
        //private void OnActiveScreenChanged(ScreenEvents.ActiveScreenChanged e)
        //{
        //    //TODO: The presenter shouldn't hard code the page here (should be agnostic), find a better way?
        //    if (e.PageName == LaJust.PowerMeter.Common.PageNames.HistoryPage)
        //    {
        //        if (_availableDates == null) UpdateCalendar();
        //        if (this.GameDate == DateTime.Today) UpdateGameDetails();
        //    }
        //}

        /// <summary>
        /// Called after [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected override void AfterPropertyChanged(string propertyName)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                switch (propertyName)
                {
                    case "SelectedGameDate": UpdateGameDetails(); break;
                    case "SelectedGameNumber": UpdateRoundDetails(); break;
                    case "SelectedRoundNumber": UpdateCompetitorList(); break;
                    case "SelectedCompetitor": UpdateCompetitorGraph(); break;
                }
            });
        }

        /// <summary>
        /// Updates the calendar.
        /// </summary>
        private void UpdateCalendar()
        {
            ReadOnlyCollection<DateTime> availableDates = GameHistoryService.GetAvailableDates();
            if (availableDates.Count > 0)
            {
                this.FirstAvailableGameDate = availableDates.Min();
            }

            this.IsDataAvailable = false;
            this.ImpactData = null;
            this.SelectedGameDate = DateTime.Today;
        }

        /// <summary>
        /// Updates the game details.
        /// </summary>
        private void UpdateGameDetails()
        {
            this.IsDataAvailable = false;

            using (new ShowBusyIndicator())
            {
                this.gameData = GameHistoryService.GetDataForDate(this.SelectedGameDate);

                if (this.gameData != null && this.gameData.Impacts.Count > 0)
                {
                    this.IsDataAvailable = true;
                    this.FirstAvailableGameNumber = this.gameData.Impacts.Min(row => row.GameNumber);
                    this.LastAvailableGameNumber = this.gameData.Impacts.Max(row => row.GameNumber);
                    this.SelectedGameNumber = this.FirstAvailableGameNumber;
                }
            }
        }

        /// <summary>
        /// Updates the round details.
        /// </summary>
        private void UpdateRoundDetails()
        {
            if (this.gameData != null)
            {
                var gameImpacts = this.gameData.Impacts.Where(row => row.GameNumber == this.SelectedGameNumber);
                if (gameImpacts.Count() > 0)
                {
                    this.LastAvailableRoundNumber = gameImpacts.Max(row => row.RoundNumber);
                    this.SelectedRoundNumber = gameImpacts.Min(row => row.RoundNumber);
                }
                else
                {
                    this.LastAvailableRoundNumber = 1;
                    this.SelectedRoundNumber = 1;
                }
            };
        }

        /// <summary>
        /// Updates the competitor list.
        /// </summary>
        private void UpdateCompetitorList()
        {
            string prevSelectedName = this.SelectedCompetitor;
            this.CompetitorNames = null;
            this.SelectedCompetitor = null;

            if (this.gameData != null)
            {
                using (new ShowBusyIndicator())
                {
                    var names = from impact in this.gameData.Impacts
                                where impact.GameNumber == this.SelectedGameNumber &&
                                      impact.RoundNumber == this.SelectedRoundNumber
                                orderby impact.Name ascending
                                group impact by impact.Name
                                    into name
                                    select name.Key;

                    if (names.Count() > 0)
                    {
                        this.CompetitorNames = new DispatchingObservableCollection<string>(names);
                        this.SelectedCompetitor = names.Contains(prevSelectedName) ? prevSelectedName : names.First();
                    }
                }
            };
        }

        /// <summary>
        /// Updates the competitor graph.
        /// </summary>
        private void UpdateCompetitorGraph()
        {
            if (this.gameData != null)
            {
                using (new ShowBusyIndicator())
                {
                    this.Title = string.Empty;
                    this.ImpactData =
                        from impact in this.gameData.Impacts
                        where impact.GameNumber == this.SelectedGameNumber &&
                              impact.RoundNumber == this.SelectedRoundNumber &&
                              impact.Name == this.SelectedCompetitor
                        select impact;

                    var scores = from score in this.gameData.Scores
                                 where score.GameNumber == this.SelectedGameNumber &&
                                       score.RoundNumber == this.SelectedRoundNumber &&
                                       score.Name == this.SelectedCompetitor
                                 select score;

                    if (this.ImpactData.Count() > 0)
                    {
                        this.Title = string.Format(
                            "{0} ({1} Game {2:D3} {3}R Score {4})",
                            this.ImpactData.First().Name,
                            this.ImpactData.First().Timestamp.ToShortDateString(),
                            this.ImpactData.First().GameNumber,
                            this.ImpactData.First().RoundNumber,
                            scores.LastOrDefault() != null ? scores.Last().NewScore : 0
                        );
                    }
                }
            }
        }

        #endregion
    }
}
