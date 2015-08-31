/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Config
{
    using LaJust.PowerMeter.Common;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using LaJust.PowerMeter.Common.BaseClasses;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Modules.Config.Views;
    using LaJust.PowerMeter.Modules.Config.Presenters;

    [Module(ModuleName = "Config")]
    public class ConfigModule : IModule
    {
        private readonly IRegionViewRegistry _regionRegistry;
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        public ConfigModule(IRegionViewRegistry regionRegistry, IUnityContainer container)
        {
            _regionRegistry = regionRegistry;
            _container = container;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<SockGloveModeView>();
                Presenter presenter = _container.Resolve<ConfigPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<ConfigRequiredImpact>();
                Presenter presenter = _container.Resolve<ConfigPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<RegisterDeviceView>();
                Presenter presenter = _container.Resolve<ConfigPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            //_regionRegistry.RegisterViewWithRegion(RegionNames.ConfigImpactRequiredRegion, () =>
            //{
            //    IView view = _container.Resolve<ImpactRequiredView>();
            //    Presenter presenter = _container.Resolve<ConfigPresenter>();
            //    view.ApplyPresenter(presenter);
            //    return view;
            //});
        }

        #endregion
    }
}
