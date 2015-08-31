/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Roster.Presenters
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using LaJust.PowerMeter.Modules.Roster.Models;
    using LaJust.PowerMeter.Modules.Roster.Services;

    public class RosterPresenter : Presenter
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
            set { _selectedName = value; }
        }

        #endregion

        #region Public Commands

        public ICommand AddCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RosterPresenter"/> class.
        /// </summary>
        public RosterPresenter(RosterService service)
        {
            _service = service;
            RosterNames = service.Competitors;
            InitializeCommands();
        }

        ~RosterPresenter()
        {
            _service.SaveCompetitors();
        }

        private void InitializeCommands()
        {
            AddCommand = new DelegateCommand<object>(o => 
            {
                RosterNames.Add(new Competitor() { Name = "< A New Competitor >" });
            });

            DeleteCommand = new DelegateCommand<object>(o => { if (_selectedName != null) RosterNames.Remove(_selectedName); });
        }

        #endregion
    }
}
