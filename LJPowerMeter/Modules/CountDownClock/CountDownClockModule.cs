/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.CountDownClock
{
    using LaJust.PowerMeter.Modules.CountDownClock.Presenters;
    using LaJust.PowerMeter.Modules.CountDownClock.Views;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Models;
    using System.Windows.Controls;

    [Module(ModuleName = "CountDownClock")]
    public class CountDownClockModule : IModule
    {
        private readonly IRegionViewRegistry _regionRegistry;
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        public CountDownClockModule(IUnityContainer container, IRegionViewRegistry regionRegistry)
        {
            _regionRegistry = regionRegistry;
            _container = container;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module to initialize itself.
        /// </summary>
        public void Initialize()
        {
            // Register presenters as singletons so we have only one even if there are multiple displays
            _container.RegisterType<CountDownClockPresenter>(new ContainerControlledLifetimeManager());

            // Register our clock region
            _regionRegistry.RegisterViewWithRegion(RegionNames.ClockRegion, () =>
                {
                    IView view = _container.Resolve<CountDownClockView>();
                    Presenter presenter = _container.Resolve<CountDownClockPresenter>();
                    view.ApplyPresenter(presenter);
                    return view;
                });

            // Register our clock config region
            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<ConfigDurationView>();
                Presenter presenter = _container.Resolve<ConfigPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

        }

        #endregion
    }
}
