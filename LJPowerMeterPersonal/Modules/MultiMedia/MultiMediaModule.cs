/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.MultiMedia
{
    using System;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Modules.MultiMedia.Services;
    using Microsoft.Practices.Composite.Regions;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Modules.MultiMedia.Views;
    using LaJust.PowerMeter.Modules.MultiMedia.Presenters;

    [Module(ModuleName = "MultiMedia")]
    public class MultiMediaModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionViewRegistry _regionRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverModule"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MultiMediaModule(IUnityContainer container, IRegionViewRegistry regionRegistry)
        {
            _container = container;
            _regionRegistry = regionRegistry;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            IEventAggregator eventAggregator = _container.Resolve<IEventAggregator>();

            Registration();

            // Schedule additional initializtion work to be done after all modules have initialized
            _container.Resolve<IEventAggregator>()
               .GetEvent<ProcessEvent>().Subscribe(new Action<ProcessEventType>(p =>
               {
                   if (p == ProcessEventType.ModulesInitialized) OnModulesInitialized();
               }),  true);
        }

        #endregion

        /// <summary>
        /// Registration for this module.
        /// </summary>
        private void Registration()
        {
            _container.RegisterType<SoundEffectsService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<SpeechService>(new ContainerControlledLifetimeManager());

            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<ConfigView>();
                Presenter presenter = _container.Resolve<ConfigPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });
        }

        /// <summary>
        /// Called when all modules have completed initializing.
        /// </summary>
        private void OnModulesInitialized()
        {
            _container.Resolve<SoundEffectsService>();
            _container.Resolve<SpeechService>();
        }

    }
}
