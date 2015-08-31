// <copyright file="PopupGraphViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Graphing
{
    using System;
    using System.Windows.Input;
    using Infrastructure;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Commands;

    public class PopupGraphViewModel : BaseViewModel
    {
        private readonly IScoreKeeperService ScoreKeeperService;

        private string sensorId;

        private string title;

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return this.title; }
            private set { this.title = value; RaisePropertyChanged("Title"); }
        }

        /// <summary>
        /// Gets the impact history.
        /// </summary>
        /// <value>The impact history.</value>
        public DispatchingObservableCollection<byte> ImpactHistory
        {
            get { return this.ScoreKeeperService[this.sensorId].ImpactHistory; }
        }

        /// <summary>
        /// Gets or sets the meter impacts.
        /// </summary>
        /// <value>The meter impacts.</value>

        #region Public Commands

        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterGraphPresenter"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public PopupGraphViewModel(IEventAggregator eventAggregator, ILoggerFacade logger, IScoreKeeperService scoreKeeperService) : base(eventAggregator, logger)
        {
            this.ScoreKeeperService = scoreKeeperService;
            this.CloseCommand = new DelegateCommand<object>(new Action<object>(o => OnRequestRemoval() ));
        }

        /// <summary>
        /// Configures the graph presenter.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="sensorId">The sensor id.</param>
        public void Configure(string sensorId)
        {
            this.sensorId = sensorId;
            this.Title = string.Format(
                "{0} (Game {1:D3} {2}R Score {3})",
                this.ScoreKeeperService[this.sensorId].DisplayName,
                this.ScoreKeeperService.GameNumber,
                this.ScoreKeeperService.RoundNumber,
                this.ScoreKeeperService[this.sensorId].Score);
            RaisePropertyChanged("ImpactHistory");
        }
    }
}
