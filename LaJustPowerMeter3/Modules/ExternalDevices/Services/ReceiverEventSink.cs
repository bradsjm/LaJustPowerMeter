// <copyright file="ReceiverController.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ExternalDevices
{
    using System;
    using System.Linq;
    using System.ComponentModel.Composition;
    using Infrastructure;
    using LaJust.EIDSS.Communications;
    using Microsoft.Practices.Prism.ViewModel;
    using LaJust.EIDSS.Communications.Entities;

    /// <summary>
    /// Receiver Handler for the PowerMeter application
    /// </summary>
    [Export(typeof(IReceiverEventSink))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ReceiverEventSink : IReceiverEventSink
    {
        #region Protected Fields

        /// <summary>
        /// Receiver Manager
        /// </summary>
        private IReceiverManager receiverManager;

        private ReceiverEvents.RegisterDevice registerDeviceRequest;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverEventSink"/> class.
        /// </summary>
        /// <param name="registerDeviceRequest">The register device request.</param>
        [ImportingConstructor]
        public ReceiverEventSink(ReceiverEvents.RegisterDevice registerDeviceRequest)
        {
            this.registerDeviceRequest = registerDeviceRequest;
        }

        #endregion

        #region Public Properties

        /// <summary>

        #endregion

        #region Public Methods

        public void Run(IReceiverManager receiverManager)
        {
            this.receiverManager = receiverManager;
            
            this.registerDeviceRequest.Subscribe(
                registration =>
                {
                    foreach (var receiver in this.receiverManager.GetReceivers())
                    {
                        receiver.RegisterDevice(registration);
                    }
                });
        }

        #endregion
    }
}
