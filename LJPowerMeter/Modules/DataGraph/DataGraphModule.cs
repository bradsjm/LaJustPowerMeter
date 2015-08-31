/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
using System;
 */
namespace LaJust.PowerMeter.Modules.DataGraph
{
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Modules.DataGraph.Models;
    using LaJust.PowerMeter.Modules.DataGraph.Presenters;
    using LaJust.PowerMeter.Modules.DataGraph.Services;
    using LaJust.PowerMeter.Modules.DataGraph.Views;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Composite.Events;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using System;
    using LaJust.PowerMeter.Common.Helpers;

    [Module(ModuleName = "DataGraph")]
    public class DataGraphModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        public DataGraphModule(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            Registration();

            // Initialize the singleton ReceiverService for the lifetime of the container
            _container.Resolve<DataLogService>();

            _eventAggregator.GetEvent<DataGraphEvents.ShowMeterHistory>().Subscribe(OnMeterShowPopupGraphEvent, ThreadOption.UIThread, true);
        }

        #endregion

        /// <summary>
        /// Registration for this module.
        /// </summary>
        private void Registration()
        {
            _container.RegisterType<DataLogModel>(new ContainerControlledLifetimeManager());
            _container.RegisterType<DataLogService>(new ContainerControlledLifetimeManager());

            _regionManager.RegisterViewWithRegion(RegionNames.HistoryGraphSelectors, () =>
            {
                IView view = _container.Resolve<GameSelectorView>();
                Presenter presenter = _container.Resolve<GameSelectorPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            _regionManager.RegisterViewWithRegion(RegionNames.HistoryGraphMain, () =>
            {
                IView view = _container.Resolve<GameImpactGraphView>();
                Presenter presenter = _container.Resolve<GameImpactGraphPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

        }

        /// <summary>
        /// Called when [meter show popup graph event].
        /// </summary>
        /// <param name="meterShowPopupGraphData">The meter show popup graph data.</param>
        private void OnMeterShowPopupGraphEvent(DataGraphEvents.ShowMeterHistory e)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OverlayRegion, () =>
            {
                using (new ShowBusyIndicator())
                {
                    MeterGraphPopupView view = _container.Resolve<MeterGraphPopupView>();
                    MeterGraphPresenter presenter = _container.Resolve<MeterGraphPresenter>();
                    presenter.Configure(e.DisplayName, e.SensorId);
                    view.ApplyPresenter(presenter);
                    presenter.RequestRemoval += (sender, args) => _regionManager.Regions[RegionNames.OverlayRegion].Remove(view);
                    return view;
                }
            });
        }
    }
}
