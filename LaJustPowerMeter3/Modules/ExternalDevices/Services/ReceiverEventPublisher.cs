// <copyright file="ReceiverEventPublisher.cs" company="LaJust Sports America">
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
    [Export(typeof(IReceiverEventPublisher))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ReceiverEventPublisher : NotificationObject, IDisposable, IReceiverEventPublisher
    {
        #region Protected Fields

        /// <summary>
        /// Receiver Manager
        /// </summary>
        private IReceiverManager receiverManager;

        [Import]
        private ReceiverEvents.ButtonPressed buttonPressedEvent = null;

        [Import]
        private ReceiverEvents.DeviceRegistered deviceRegisteredEvent = null;

        [Import]
        private ReceiverEvents.DeviceStatusUpdate deviceStatusUpdateEvent = null;

        [Import]
        private ReceiverEvents.SensorHit sensorHitEvent = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the receiver count.
        /// </summary>
        /// <value>The receiver count.</value>
        public int ReceiverCount
        {
            get { return this.receiverManager == null ? 0 : this.receiverManager.Count(); }
        }

        #endregion

        #region Public Methods

        public void Run(IReceiverManager receiverManager)
        {
            this.receiverManager = receiverManager;
            this.receiverManager.PanelButtonPressed += (sender, e) => buttonPressedEvent.Publish(e);
            this.receiverManager.StrikeDetected += (sender, e) => sensorHitEvent.Publish(e);
            this.receiverManager.DeviceRegistered += (sender, e) => deviceRegisteredEvent.Publish(e);
            this.receiverManager.DeviceStatusUpdate += (sender, e) => { deviceStatusUpdateEvent.Publish(e); ActivitySimulator.MoveMouse(); };
            this.receiverManager.ReceiverCountChanged += (sender, e) => RaisePropertyChanged(() => this.ReceiverCount);

            RaisePropertyChanged(() => this.ReceiverCount);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            if (this.receiverManager != null)
            {
                this.receiverManager.PanelButtonPressed -= (sender, e) => buttonPressedEvent.Publish(e);
                this.receiverManager.StrikeDetected -= (sender, e) => sensorHitEvent.Publish(e);
                this.receiverManager.DeviceRegistered -= (sender, e) => deviceRegisteredEvent.Publish(e);
                this.receiverManager.DeviceStatusUpdate -= (sender, e) => deviceStatusUpdateEvent.Publish(e);
                this.receiverManager.ReceiverCountChanged -= (sender, e) => RaisePropertyChanged(() => this.ReceiverCount);
                this.receiverManager.Dispose();
            }
        }

        #if DEBUG
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ReceiverManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~ReceiverEventPublisher()
        {
            throw new InvalidOperationException("ReceiverEventPublisher Dispose method not called.");
        }
        #endif

        #endregion
    }
}
