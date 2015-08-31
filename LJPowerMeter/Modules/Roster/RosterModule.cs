/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
using System;
 */
namespace LaJust.PowerMeter.Modules.Roster
{
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Modules.Roster.Presenters;
    using LaJust.PowerMeter.Modules.Roster.Views;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Common.Helpers;
    using LaJust.PowerMeter.Modules.Roster.Services;

    [Module(ModuleName = "Roster")]
    public class RosterModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RosterModule"/> class.
        /// </summary>
        /// <param name="regionRegistry">The region registry.</param>
        /// <param name="container">The container.</param>
        public RosterModule(IRegionManager regionManager, IUnityContainer container, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _container = container;
            _eventAggregator = eventAggregator;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            _container.RegisterType<RosterService>(new ContainerControlledLifetimeManager());

            _eventAggregator.GetEvent<RosterEvents.ShowRosterPickList>().Subscribe(OnShowRosterPickList, ThreadOption.UIThread, true);

            _regionManager.RegisterViewWithRegion(RegionNames.RosterNamesRegion, () =>
            {
                IView view = _container.Resolve<RosterView>();
                Presenter presenter = _container.Resolve<RosterPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });
        }

        #endregion

        /// <summary>
        /// Called when [meter show roster event handler].
        /// </summary>
        /// <param name="sensorId">The sensor id.</param>
        private void OnShowRosterPickList(RosterEvents.ShowRosterPickList e)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.OverlayRegion, () =>
            {
                using (new ShowBusyIndicator())
                {
                    IView view = _container.Resolve<RosterNamesPopupView>();
                    RosterNamesPopupPresenter presenter = _container.Resolve<RosterNamesPopupPresenter>();
                    view.ApplyPresenter(presenter);
                    presenter.RequestRemoval += (sender, args) =>
                    {
                        if (e.ResultHandler != null && presenter.SelectedName != null) 
                            e.ResultHandler.BeginInvoke(presenter.SelectedName.Name, null, null);
                        _regionManager.Regions[RegionNames.OverlayRegion].Remove(view);
                    };
                    return view;
                }
            });
        }
    }
}
