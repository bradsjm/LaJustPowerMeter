// <copyright file="ExternalDevicesModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ExternalDevices
{
    using Infrastructure;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;
    using System.ComponentModel.Composition;
    using System.Windows;
    using LaJust.EIDSS.Communications;

    /// <summary>
    /// 
    /// </summary>
    [ModuleExport(typeof(ExternalDevicesModule))]
    public sealed class ExternalDevicesModule : IModule
    {
        private IAppConfigService appConfigService;

        private IReceiverEventSink receiverEventSink;

        private IReceiverEventPublisher receiverEventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalDevicesModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public ExternalDevicesModule(
            IAppConfigService appConfigService,
            IReceiverEventSink receiverEventSink,
            IReceiverEventPublisher receiverEventPublisher
            )
        {
            this.appConfigService = appConfigService;
            this.receiverEventSink = receiverEventSink;
            this.receiverEventPublisher = receiverEventPublisher;
        }
        
        /// <summary>
        /// Notifies the module to initialize.
        /// </summary>
        public void Initialize()
        {
            byte courtNumber = this.appConfigService.Settings.CourtNumber;
            IReceiverManager receiverManager = new ReceiverManager(courtNumber);
            receiverManager.ResetAll();
            this.receiverEventSink.Run(receiverManager);
            this.receiverEventPublisher.Run(receiverManager);

            //this.wiiRemoteEventPublisher.Run(WiiDeviceLibrary.DeviceProviderRegistry.CreateSupportedDeviceProvider());            
        }
    }
}