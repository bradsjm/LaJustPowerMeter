/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Receiver
{
    using System;
    using LaJust.EIDSS.Communications;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Modules.Receiver.Presenters;
    using LaJust.PowerMeter.Modules.Receiver.Services;
    using LaJust.PowerMeter.Modules.Receiver.Views;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Common.BaseClasses;

    [Module(ModuleName = "Receiver")]
    public class ReceiverModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionViewRegistry _regionRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverModule"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ReceiverModule(IUnityContainer container, IEventAggregator eventAggregator, IRegionViewRegistry regionRegistry)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionRegistry = regionRegistry;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            // Register our services into the container
            _container.RegisterType<ReceiverService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ReceiverStatePresenter>(new ContainerControlledLifetimeManager());

            // Register our clock region
            _regionRegistry.RegisterViewWithRegion(RegionNames.ReceiverStateRegion, () =>
            {
                IView view = _container.Resolve<ReceiverStateView>();
                Presenter presenter = _container.Resolve<ReceiverStatePresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            // Schedule additional initializtion work to be done after all modules have initialized
            _eventAggregator.GetEvent<ProcessEvent>().Subscribe(new Action<ProcessEventType>(p =>
               {
                   if (p == ProcessEventType.ModulesInitialized) OnModulesInitialized();
               }), true);
        }

        #endregion

        /// <summary>
        /// Called when all modules have completed initializing.
        /// </summary>
        private void OnModulesInitialized()
        {
            // Initialize the singleton ReceiverService for the lifetime of the container
            _container.Resolve<ReceiverService>();
        }

    }
}
