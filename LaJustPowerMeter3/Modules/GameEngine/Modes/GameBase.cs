// <copyright file="GameBase.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Infrastructure;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Regions;

    public abstract class GameBase
    {
        #region Protected Member Fields

        /// <summary>
        /// Composite Application Library Logger
        /// </summary>
        [Import]
        protected ILoggerFacade Logger;

        /// <summary>
        /// Application Configuration
        /// </summary>
        [Import]
        protected IAppConfigService AppConfigService;

        /// <summary>
        /// Application Score Keeper Service (also tracks game and round numbers)
        /// </summary>
        [Import]
        protected IScoreKeeperService ScoreKeeperService;

        /// <summary>
        /// The Prism Region Manager
        /// </summary>
        [Import]
        protected IRegionManager RegionManager;

        /// <summary>
        /// The Stop Watch Service providing game timings
        /// </summary>
        [Import]
        protected IStopWatchService StopWatchService;

        protected Dictionary<ToolBarItemModel, string> ToolBarItems = new Dictionary<ToolBarItemModel, string>();

        protected Dictionary<SubscriptionToken, EventBase> SubscriptionTokens = new Dictionary<SubscriptionToken, EventBase>();

        #endregion

        /// <summary>
        /// Adds the subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="token">The token.</param>
        /// <returns>The token.</returns>
        protected SubscriptionToken AddSubscription(EventBase subscription, SubscriptionToken token)
        {
            this.SubscriptionTokens.Add(token, subscription);
            return token;
        }

        /// <summary>
        /// Adds the toolbar item.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected ToolBarItemModel AddToolbarItem(string region, string text, string icon, DelegateCommand command)
        {
            ToolBarItemModel toolBarItem = new ToolBarItemModel() { Text = text, Icon = icon, Command = command };
            this.ToolBarItems.Add(toolBarItem, region);
            this.RegionManager.AddToRegion(region, toolBarItem);
            return toolBarItem;
        }

        /// <summary>
        /// Refreshes the toolbar.
        /// </summary>
        protected void RefreshToolbar()
        {
            foreach (var item in this.ToolBarItems.Keys)
            {
                item.Command.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Clears the toolbar.
        /// </summary>
        protected void ClearToolbar()
        {
            foreach (var item in this.ToolBarItems)
            {
                this.RegionManager.Regions[item.Value].Remove(item.Key);
            }
            this.ToolBarItems.Clear();
        }

        /// <summary>
        /// Unsubscribes all events
        /// </summary>
        protected void UnsubscribeAll()
        {
            foreach (var token in this.SubscriptionTokens)
            {
                token.Value.Unsubscribe(token.Key);
            }
            this.SubscriptionTokens.Clear();
        }

        #region Public Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Stops this instance and unsubscribes from events
        /// </summary>
        public void Stop()
        {
            this.UnsubscribeAll();
            this.ClearToolbar();
        }

        #endregion
    }
}