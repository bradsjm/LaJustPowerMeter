// <copyright file="RosterViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Roster
{
    using System.Windows.Input;
    using Infrastructure;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Roster View Model
    /// </summary>
    public class RosterViewModel
    {
        #region Private Fields

        /// <summary>
        /// Reference to the Roster Service
        /// </summary>
        private readonly RosterService RosterService;

        /// <summary>
        /// Backing field for the Selected Name of the Competitor
        /// </summary>
        private CompetitorModel selectedName;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RosterViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="rosterService">The roster service.</param>
        public RosterViewModel(IEventAggregator eventAggregator, ILoggerFacade logger, RosterService rosterService)
        {
            this.RosterService = rosterService;
            this.RegisterHandlers();

            logger.Log("RosterViewModel Initialized", Category.Debug, Priority.None);
        }

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets the add command.
        /// </summary>
        /// <value>The add command.</value>
        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        /// <value>The delete command.</value>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Gets the roster names collection.
        /// </summary>
        /// <value>The roster names.</value>
        public DispatchingObservableCollection<CompetitorModel> RosterNames
        {
            get { return this.RosterService.Competitors; }
        }

        /// <summary>
        /// Gets or sets the name of the selected.
        /// </summary>
        /// <value>The name of the selected.</value>
        public CompetitorModel SelectedName
        {
            get { return this.selectedName; }
            set { this.selectedName = value; }
        }

        #endregion

        /// <summary>
        /// Registers the handlers.
        /// </summary>
        private void RegisterHandlers()
        {
            this.AddCommand = new DelegateCommand<object>(
                o => this.RosterNames.Add(new CompetitorModel() { DisplayName = "* A New Competitor *" }));

            this.DeleteCommand = new DelegateCommand<object>(
                delegate
                {
                    if (this.selectedName != null)
                    {
                        this.RosterNames.Remove(this.selectedName);
                    }
                });
        }
    }
}
