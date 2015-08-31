// <copyright file="RosterNamesPopupViewModel.cs" company="LaJust Sports America">
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
    using System.ComponentModel.Composition;

    /// <summary>
    /// Roster Names Popup View Model
    /// </summary>
    [Export]
    public class RosterNamesPopupViewModel
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
        /// Initializes a new instance of the <see cref="RosterNamesPopupViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="rosterService">The roster service.</param>
        [ImportingConstructor]
        public RosterNamesPopupViewModel(RosterService rosterService)
        {
            this.RosterService = rosterService;
            //this.RosterNames.CollectionChanged += (s, e) => RaisePropertyChanged("RosterNames");
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// Gets the close popup command.
        /// </summary>
        /// <value>The close popup command.</value>
        public ICommand CloseCommand { get; private set; }

        #endregion

        #region Public Binding Properties

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
            get
            {
                return this.selectedName; 
            }

            set 
            {
                this.selectedName = value; 
                //OnRequestRemoval(); 
            }
        }

        #endregion
    }
}
