// <copyright file="PopupController.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>using System;

namespace Roster
{
    using System.Linq;
    using Infrastructure;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.Regions;

    /// <summary>
    /// Meter Controller responsible for adding/removing meters on demand
    /// </summary>
    public class PopupController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PopupController"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="container">The container.</param>
        public PopupController(IEventAggregator eventAggregator, ILoggerFacade logger, IRegionManager regionManager)
        {
            this.RegisterHandlers();
        }

        /// <summary>
        /// Registers the handlers.
        /// </summary>
        protected void RegisterHandlers()
        {
        }

        /// <summary>
        /// Called when [show pop up].
        /// </summary>
        /// <param name="showPopupEvent">The show popup event.</param>
            //IRegion popupRegion = this.RegionManager.Regions[RegionNames.OverlayRegion];

            //if (popupRegion.ActiveViews.Count() == 0)
            //{
            //    this.RegionManager.RegisterViewWithRegion(
            //        RegionNames.OverlayRegion,
            //        delegate
            //        {
            //            var view = this.Container.Resolve<RosterNamesPopupView>();
            //            var viewModel = this.Container.Resolve<RosterNamesPopupViewModel>();
            //            view.ApplyViewModel(viewModel);
            //            viewModel.RequestRemoval += (s, e) =>
            //            {
            //                if (viewModel.SelectedName is CompetitorModel)
            //                {
            //                    showPopupEvent.ResultHandler.Invoke(viewModel.SelectedName.DisplayName);
            //                }
            //                popupRegion.Remove(view);
            //            };

            //            return view;
            //        });
            //}
    }
}
