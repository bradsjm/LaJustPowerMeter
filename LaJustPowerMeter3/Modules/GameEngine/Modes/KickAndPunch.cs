// <copyright file="WetBagGameService.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Infrastructure;
    using LaJust.EIDSS.Communications.Entities;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Events;

    [Export]
    public class KickAndPunch : GameBase
    {
        #region Private Member Fields

        private readonly ReceiverEvents.ButtonPressed buttonPressedEvent;

        private readonly ReceiverEvents.DeviceRegistered deviceRegisteredEvent;
        
        private readonly ReceiverEvents.SensorHit deviceSensorHit;

        private readonly ReceiverEvents.DeviceStatusUpdate deviceStatusNotify;

        private readonly ReceiverEvents.RegisterDevice registerDeviceRequest;

        private readonly ApplicationEvents.ApplicationClosing appClosingEvent;

        /// <summary>
        /// Tool Bar Region for Game Tool Bar Buttons
        /// </summary>
        private const string TOOLBARREGION = RegionNames.ToolbarLeftRegion;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public KickAndPunch(
            ReceiverEvents.DeviceRegistered deviceRegisteredEvent,
            ReceiverEvents.ButtonPressed buttonPressedEvent,
            ReceiverEvents.RegisterDevice registerDeviceRequest,
            ReceiverEvents.SensorHit deviceSensorHit,
            ReceiverEvents.DeviceStatusUpdate deviceStatusNotify,
            ApplicationEvents.ApplicationClosing appClosingEvent)
        {
            this.deviceRegisteredEvent = deviceRegisteredEvent;
            this.buttonPressedEvent = buttonPressedEvent;
            this.registerDeviceRequest = registerDeviceRequest;
            this.deviceSensorHit = deviceSensorHit;
            this.deviceStatusNotify = deviceStatusNotify;
            this.appClosingEvent = appClosingEvent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Run()
        {
            this.StopWatchService.Duration = this.AppConfigService.Settings.RoundDuration;
            this.StopWatchService.ResetCountDown();

            this.ScoreKeeperService.RestoreCompetitorState();
            this.MatchPartners();
            appClosingEvent.Subscribe((e) => this.ScoreKeeperService.PersistCompetitorState());

            this.ScoreKeeperService.GameNumber++;

            AddSubscription(this.buttonPressedEvent, 
                this.buttonPressedEvent.Subscribe( b => this.OnReceiverButtonPress(b.Button)));

            AddSubscription(this.deviceSensorHit,
                this.deviceSensorHit.Subscribe( d => this.OnSensorHit(d)));

            AddSubscription(this.deviceStatusNotify,
                this.deviceStatusNotify.Subscribe(d => this.OnStatusNotification(d)));

            AddSubscription(this.deviceRegisteredEvent, 
                this.deviceRegisteredEvent.Subscribe( d => this.OnDeviceRegistration(d), ThreadOption.UIThread));

            AddToolbarItem(TOOLBARREGION, "Start", "/GameEngine;component/Resources/Buttons/Start.png",
                new DelegateCommand(() => this.OnReceiverButtonPress(PanelButtons.Start), () => !this.StopWatchService.IsRunning)
            );

            AddToolbarItem(TOOLBARREGION, "Pause", "/GameEngine;component/Resources/Buttons/Stop.png",
                new DelegateCommand(() => this.OnReceiverButtonPress(PanelButtons.Clocking), () => this.StopWatchService.IsRunning)
            );

            AddToolbarItem(TOOLBARREGION, "Reset", "/GameEngine;component/Resources/Buttons/Reset.png",
                new DelegateCommand(() => this.OnReceiverButtonPress(PanelButtons.Clocking), () => !this.StopWatchService.IsRunning)
            );

            this.Logger.Log("KickAndPunch game module started", Category.Info, Priority.Low);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when a device is sucessfully registered to the receiver. We need to add
        /// or update it with the scoreekeeper competitors service.
        /// </summary>
        /// <param name="deviceRegistration">The device registration.</param>
        protected void OnDeviceRegistration(DeviceRegistrationEventArgs deviceRegistration)
        {
            var competitor = this.ScoreKeeperService.Competitors.FirstOrDefault(c => c.DeviceId.Equals(deviceRegistration.Id));

            if (competitor != null)
            {
                competitor.DeviceType = deviceRegistration.OperationCode;
            }
            else
            {
                this.ScoreKeeperService.Competitors.Add(new CompetitorModel(deviceRegistration));
            }

            // Attempt to match up partners
            this.MatchPartners();
        }

        /// <summary>
        /// Matches the partner.
        /// </summary>
        protected void MatchPartners()
        {
            // Iterate through each competitor that does not have a partner yet
            foreach (CompetitorModel competitor in this.ScoreKeeperService.Competitors.Where(c => !c.HasPartner))
            {
                OpCodes partnerOpCode;

                // Hong partners with Chung and vice-versa
                switch (competitor.DeviceType)
                {
                    case OpCodes.ChungRegistered: partnerOpCode = OpCodes.HongRegistered; break;
                    case OpCodes.HongRegistered:  partnerOpCode = OpCodes.ChungRegistered; break;
                    default: continue; // skip anything we don't have a rule for
                }

                // Search for a matching competitor
                CompetitorModel partner = this.ScoreKeeperService.Competitors.FirstOrDefault(c => !c.HasPartner && c.DeviceType == partnerOpCode);

                // Assign the partners if we were successful
                if (partner != null)
                {
                    competitor.Partner = partner; // The model will do the work of reverse mating
                    this.Logger.Log("KickAndPunch matched " + competitor.DisplayName + " with " + partner.DisplayName, Category.Info, Priority.Low);
                }
            }
        }

        /// <summary>
        /// Called when a button is pressed on the receiver.
        /// </summary>
        /// <param name="button">The receiver button pressed.</param>
        protected void OnReceiverButtonPress(PanelButtons button)
        {
            switch (button)
            {
                case PanelButtons.Start:
                    if (!this.StopWatchService.IsRunning)
                    {
                        this.StopWatchService.StartCountDown();
                    }
                    break;

                case PanelButtons.Clocking:
                    if (this.StopWatchService.IsRunning)
                    {
                        this.StopWatchService.PauseCountDown();
                    }
                    else
                    {
                        this.StopWatchService.Duration = this.AppConfigService.Settings.RoundDuration;
                        this.ScoreKeeperService.ResetScores();
                    }

                    break;

                case PanelButtons.ChungRegister:
                case PanelButtons.HongRegister:
                    this.registerDeviceRequest.Publish(new RegistrationSettings()
                    {
                        GameNumber = 1,
                        MinimumPressure = 30,
                        OperationCode = (button == PanelButtons.ChungRegister) ? OpCodeCmds.RegisterChung : OpCodeCmds.RegisterHong,
                        TouchSensorMode = this.AppConfigService.Settings.ContactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired
                    });
                    break;

                case PanelButtons.HongWin:
                    this.registerDeviceRequest.Publish(new RegistrationSettings()
                    {
                        GameNumber = 1,
                        MinimumPressure = 30,
                        OperationCode = OpCodeCmds.RegisterTarget,
                        TouchSensorMode = this.AppConfigService.Settings.ContactSensorRequired ? TouchSensorStatusEnum.Required : TouchSensorStatusEnum.NotRequired
                    });
                    break;
            }
            this.RefreshToolbar();
        }

        /// <summary>
        /// Called to change the required impact level by the given delta amount
        /// </summary>
        /// <param name="delta">The delta.</param>
        protected void OnChangeRequiredImpactLevel(int delta)
        {
            if (!this.StopWatchService.IsRunning)
            {
                foreach (var competitor in this.ScoreKeeperService.Competitors)
                {
                    competitor.RequiredImpactLevel = (byte)(competitor.RequiredImpactLevel + delta);
                }

                this.AppConfigService.Settings.RequiredImpactLevel = (byte)(this.AppConfigService.Settings.RequiredImpactLevel + delta);
            }
        }

        /// <summary>
        /// Called when the receiver reports data from a device indicating a hit has registered.
        /// We need to update the scorekeeper service if a game is in progress.
        /// </summary>
        /// <param name="deviceData">The sensor impact data.</param>
        protected void OnSensorHit(DeviceDataEventArgs deviceData)
        {
            CompetitorModel competitor = this.ScoreKeeperService.Competitors.FirstOrDefault(c => c.DeviceId.Equals(deviceData.DeviceId));
            if (competitor != null)
            {
                byte impactLevel = Math.Max(deviceData.HeadHitValue, deviceData.VestHitValue);

                // If we are in partnering mode, we need to flip to award score/impact to the correct person
                if (competitor.HasPartner) competitor = competitor.Partner;

                competitor.LastImpactLevel = impactLevel;

                if (!this.StopWatchService.IsRunning && this.StopWatchService.DisplayTime == this.StopWatchService.Duration)
                {
                    competitor.Score = competitor.HighestImpactLevel;
                }
                else if (this.StopWatchService.IsRunning && impactLevel >= competitor.RequiredImpactLevel)
                {
                    competitor.Score += 1; // TODO: Allow configurable points
                    this.Logger.Log("KickAndPunch score for " + competitor.DisplayName + " is now " + competitor.Score, Category.Info, Priority.Low);
                }
            }
        }

        /// <summary>
        /// Called when the receiver sends a status packet such as low battery or lost connection to us
        /// </summary>
        /// <param name="deviceStatus">The <see cref="LaJust.EIDSS.Communications.Entities.DeviceStatusEventArgs"/> instance containing the event data.</param>
        protected void OnStatusNotification(DeviceStatusEventArgs deviceStatus)
        {
            CompetitorModel competitor = this.ScoreKeeperService.Competitors.FirstOrDefault(c => c.DeviceId.Equals(deviceStatus.DeviceId));

            if (competitor != null)
            {
                competitor.DeviceStatus = deviceStatus.DeviceStatus;
            }
        }

        /// <summary>
        /// Called when [stop watch state changed].
        /// </summary>
        /// <param name="stopWatchService">The stop watch service.</param>
        protected void OnStopWatchStateChanged(IStopWatchService stopWatchService)
        {
            //foreach (ToolBarItemModel toolbarItem in this.RegionManager.Regions[TOOLBARREGION].ActiveViews)
            //{
            //    ((DelegateCommand<object>)toolbarItem.Command).RaiseCanExecuteChanged();
            //}

            // Check for end of countdown
            if (!stopWatchService.IsRunning && stopWatchService.DisplayTime == TimeSpan.Zero)
            {
                //this.ScoreKeeperService.IncrementRound();
            }
        }

        #endregion
    }
}