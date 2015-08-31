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
    using Microsoft.Practices.Composite.Events;
    using System.Collections.ObjectModel;
    using LaJust.PowerMeter.Modules.DataGraph.Views;
    using LaJust.PowerMeter.Common.Helpers;
    using LaJust.PowerMeter.Modules.DataGraph.Services;

    public class GameImpactGraphPresenter : Presenter
    {
        private readonly IEventAggregator _eventAggregator;
        private EnumerableRowCollection<DataLogModel.ImpactsRow> _meterImpacts;
        private string _title;
        private DelegateCommand<object> _printCommand;
        private DelegateCommand<object> _exportCommand;
        private bool _exportComplete;

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
        /// Gets or sets the meter impacts.
        /// </summary>
        /// <value>The meter impacts.</value>
        public EnumerableRowCollection<DataLogModel.ImpactsRow> MeterImpacts
        {
            get { return _meterImpacts; }
            private set { _meterImpacts = value; OnPropertyChanged("MeterImpacts"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameImpactGraphPresenter"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public GameImpactGraphPresenter(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<SelectedDataChangedEvent>().Subscribe(OnImpactDataChanged, ThreadOption.BackgroundThread);
            _printCommand = new DelegateCommand<object>(o => Print());
            _exportCommand = new DelegateCommand<object>(o => Export(), o => !_exportComplete);
        }

        /// <summary>
        /// Prints the graph.
        /// </summary>
        private void Print()
        {
            using (new ShowBusyIndicator())
            {
                GameImpactPrintView printView = new GameImpactPrintView();
                printView.ApplyPresenter(this);
                printView.Print();
            }
        }

        /// <summary>
        /// Exports the graph data.
        /// </summary>
        private void Export()
        {
            using (new ShowBusyIndicator())
            {
                ExcelService excelService = new ExcelService();
                excelService.Export(this.MeterImpacts);
                _exportComplete = true;
                _exportCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Called when [impact data changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnImpactDataChanged(SelectedDataChangedEvent e)
        {
            _exportComplete = false;
            _exportCommand.RaiseCanExecuteChanged();

            if (e.ImpactData.Count() < 1)
            {
                Title = string.Empty;
                MeterImpacts = null;
            }
            else
            {
                Title = string.Format("{0} ({1} Game {2:D3} {3}R Score {4})", 
                    e.ImpactData.First().Name, 
                    e.ImpactData.First().Timestamp.ToShortDateString(), 
                    e.ImpactData.First().GameNumber, 
                    e.ImpactData.First().RoundNumber,
                    (e.ScoreData.Count() > 0 ? e.ScoreData.Last().NewScore : 0)
                    );
                MeterImpacts = e.ImpactData;
            }

        }

    }
}
