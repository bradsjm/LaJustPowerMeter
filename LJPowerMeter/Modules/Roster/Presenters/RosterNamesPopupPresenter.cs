/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Roster.Presenters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using LaJust.PowerMeter.Modules.Roster.Services;
    using LaJust.PowerMeter.Modules.Roster.Models;

    public class RosterNamesPopupPresenter : Presenter
    {
        #region Private Fields

        private RosterService _service;

        private Competitor _selectedName;

        #endregion

        #region Public Binding Properties

        /// <summary>
        /// Gets or sets the roster names.
        /// </summary>
        /// <value>The roster names.</value>
        public ObservableCollection<Competitor> RosterNames { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected.
        /// </summary>
        /// <value>The name of the selected.</value>
        public Competitor SelectedName
        {
            get { return _selectedName; }
            set { _selectedName = value; OnRequestRemoval(); }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigPresenter"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public RosterNamesPopupPresenter(RosterService service)
        {
            _service = service;
            CloseCommand = new DelegateCommand<object>(o => OnRequestRemoval());
            RosterNames = service.Competitors;
        }

        #endregion
    }
}
