/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Meter
{
    using System.Collections.Generic;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Events;
    using LaJust.PowerMeter.Modules.Meter.Presenters;
    using LaJust.PowerMeter.Modules.Meter.Views;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Events;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Common.BaseClasses;
    using LaJust.PowerMeter.Common.Helpers;
    using LaJust.PowerMeter.Common.Extensions;

    [Module(ModuleName = "Meter")]
    public class MeterModule : IModule
    {
        private const int MAX_METERS = 1;

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        private readonly List<string> _sensorIds = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        public MeterModule(IUnityContainer container, IEventAggregator eventAggregator, IRegionManager regionManager)
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
            _eventAggregator.GetEvent<ProcessEvent>().Subscribe(OnProcessEvent, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<ReceiverEvents.DeviceRegistered>().Subscribe(OnDeviceRegistered, ThreadOption.UIThread, true);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [process event].
        /// </summary>
        /// <param name="processType">Type of the process.</param>
        private void OnProcessEvent(ProcessEventType processType)
        {
            if (processType == ProcessEventType.ModulesInitialized)
            {
                _container.Resolve<MeterView>();
            }
        }

        /// <summary>
        /// Called when [sensor registered].
        /// </summary>
        /// <param name="sensorRegistrationData">The sensor registration data.</param>
        private void OnDeviceRegistered(ReceiverEvents.DeviceRegistered e)
        {
            lock (_sensorIds)
            {
                if (!_sensorIds.Contains(e.SensorId) && _sensorIds.Count < MAX_METERS)
                {
                    _sensorIds.Add(e.SensorId);
                    // NOTE: Use RegisterViewWithRegion not Add to region to fix issue with Telerik Control hanging
                    _regionManager.RegisterViewWithRegion(RegionNames.MeterRegion, () =>
                    {
                        using (new ShowBusyIndicator())
                        {
                            IView view = _container.Resolve<MeterView>();
                            MeterPresenter presenter = _container.Resolve<MeterPresenter>();
                            presenter.Id = e.SensorId;
                            presenter.SetMeterType(e.SensorType);
                            presenter.RequestRemoval += (sender, args) =>
                            {
                                lock (_sensorIds)
                                {
                                    IRegion region = _regionManager.Regions[RegionNames.MeterRegion];
                                    region.Remove(view);
                                     _sensorIds.Remove(presenter.Id);
                                }
                            };
                            view.ApplyPresenter(presenter);
                            return view;
                        }
                    });
                }
            }
        }

        #endregion
    }
}
