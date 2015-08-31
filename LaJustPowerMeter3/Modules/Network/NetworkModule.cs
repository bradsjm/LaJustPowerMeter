// <copyright file="MultiMediaModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Network
{
    using System;
    using System.ComponentModel.Composition;
    using Infrastructure;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// Network module handles connectivity to other network devices
    /// </summary>
    [ModuleExport(typeof(NetworkModule))]
    public sealed class NetworkModule : IModule
    {
        /// <summary>
        /// Application configuration settings
        /// </summary>
        private readonly IAppConfigService appConfigService;

        private readonly IScoreKeeperService scoreKeeperService;

        private readonly IStopWatchService stopWatchService;

        ReceiverEvents.ButtonPressed receiverButtonPressed;

        private MeshService networkMeshService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public NetworkModule(
            IAppConfigService appConfigService,
            IScoreKeeperService scoreKeeperService,
            IStopWatchService stopWatchService,
            ReceiverEvents.ButtonPressed receiverButtonPressed)
        {
            this.appConfigService = appConfigService;
            this.scoreKeeperService = scoreKeeperService;
            this.stopWatchService = stopWatchService;
            this.receiverButtonPressed = receiverButtonPressed;
        }

        /// <summary>
        /// Notifies the module to initialize.
        /// </summary>
        public void Initialize()
        {
            //this.networkMeshService = new MeshService();
            //this.networkMeshService.Start();
            //this.networkMeshService.Proxy.DeviceRegistered(new DeviceRegistrationEventArgs() { CourtNumber = 5 });
        }

    }
}
