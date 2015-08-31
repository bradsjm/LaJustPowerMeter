/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.GameEngine
{
    using System;
    using LaJust.PowerMeter.Common.Events;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Composite.Regions;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Models;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Modules.GameEngine.Views;
    using LaJust.PowerMeter.Modules.GameEngine.Presenters;

    [Module(ModuleName = "GameEngine")]
    public class GameEngineModule : IModule
    {
        #region Private Fields

        private readonly IUnityContainer _container;
        private IGameService _activeGameService;
        private IRegionViewRegistry _regionRegistry;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverModule"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public GameEngineModule(IUnityContainer container, IEventAggregator eventAggregator, IRegionViewRegistry regionRegistry )
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _regionRegistry = regionRegistry;
        }

        #endregion

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            Registration();

            _eventAggregator.GetEvent<ProcessEvent>().Subscribe(new Action<ProcessEventType>(p =>
               {
                   if (p == ProcessEventType.ModulesInitialized) OnModulesInitialized();
               }), true);
        }

        #endregion

        /// <summary>
        /// Registration for this module.
        /// </summary>
        private void Registration()
        {
            _container.RegisterType<IGameService, FitnessTest.FitnessTestGameService>();

            // Register our game number region
            _regionRegistry.RegisterViewWithRegion(RegionNames.GameNumberRegion, () =>
            {
                IView view = _container.Resolve<GameNumberView>();
                Presenter presenter = _container.Resolve<GameDataPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            // Register our game round number region
            _regionRegistry.RegisterViewWithRegion(RegionNames.GameRoundRegion, () =>
            {
                IView view = _container.Resolve<GameRoundView>();
                Presenter presenter = _container.Resolve<GameDataPresenter>();
                view.ApplyPresenter(presenter);
                return view;
            });

            // Register our config game number region
            _regionRegistry.RegisterViewWithRegion(RegionNames.ConfigGeneralOptionsRegion, () =>
            {
                IView view = _container.Resolve<ConfigGameNumber>();
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
            _activeGameService = _container.Resolve<IGameService>();
            _activeGameService.Start();
        }

    }
}
