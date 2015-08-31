/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.RemoteControl
{
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Modules.RemoteControl.Services;
    using Microsoft.Practices.Composite.Events;
    using LaJust.PowerMeter.Common.Events;
    using System;
    using Microsoft.Practices.Composite.Presentation.Events;

    [Module(ModuleName = "RemoteControl")]
    public class RemoteControlModule : IModule
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockModule"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        public RemoteControlModule(IUnityContainer container)
        {
            _container = container;
        }

        #region IModule Members

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            Registration();
            // Initialize the singleton ReceiverService for the lifetime of the container
            _container.Resolve<RemoteControlService>();
            #if DEBUG
            _container.Resolve<DemoService>();
            #endif
        }

        #endregion

        /// <summary>
        /// Registration for this module.
        /// </summary>
        private void Registration()
        {
            _container.RegisterType<RemoteControlService>(new ContainerControlledLifetimeManager());
            #if DEBUG
            _container.RegisterType<DemoService>(new ContainerControlledLifetimeManager());
            #endif
        }

    }
}
