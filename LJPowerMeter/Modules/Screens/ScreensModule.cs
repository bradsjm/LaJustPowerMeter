/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.Screens
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using LaJust.PowerMeter.Common;
    using LaJust.PowerMeter.Common.Helpers;
    using LaJust.PowerMeter.Common.Models;
    using Microsoft.Practices.Composite.Events;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Commands;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.Unity;
    using LaJust.PowerMeter.Common.Events;

    [Module(ModuleName = "Screens")]
    public class ScreensModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private List<ToolBarItemModel> _toolbarItems;
        private PageNames _currentScreenPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreensModule"/> class.
        /// </summary>
        public ScreensModule(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
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
            RegisterToolbarItems();
            RegisterScreenPages();
            ChangeScreenPage(RegionNames.PrimaryPageRegion, PageNames.EventPage);
            ChangeScreenPage(RegionNames.SecondaryPageRegion, PageNames.EventPage);
        }

        #endregion

        /// <summary>
        /// Registers the screen pages.
        /// </summary>
        private void RegisterScreenPages()
        {
            // Register the primary screen page views
            _regionManager.RegisterViewWithRegion(RegionNames.PrimaryPageRegion, typeof(Pages.Event));
            _regionManager.RegisterViewWithRegion(RegionNames.PrimaryPageRegion, typeof(Pages.Config));
            _regionManager.RegisterViewWithRegion(RegionNames.PrimaryPageRegion, typeof(Pages.Roster));
            _regionManager.RegisterViewWithRegion(RegionNames.PrimaryPageRegion, typeof(Pages.History));

            // Register the secondary screen (e.g. projector) page view
            _regionManager.RegisterViewWithRegion(RegionNames.SecondaryPageRegion, typeof(Pages.Event));
        }

        /// <summary>
        /// Registers the toolbar items.
        /// </summary>
        private void RegisterToolbarItems()
        {
            _toolbarItems = new List<ToolBarItemModel>()
            {
                new ToolBarItemModel()
                {
                    Text = "Event",
                    Icon = "/LaJust.PowerMeter.Modules.Screens;component/Resources/Clock.png",
                    Command = new DelegateCommand<object>(
                        o => ChangeScreenPage(RegionNames.PrimaryPageRegion, PageNames.EventPage),
                        o => _currentScreenPage != PageNames.EventPage )
                },

                new ToolBarItemModel()
                {
                    Text = "Setup",
                    Icon = "/LaJust.PowerMeter.Modules.Screens;component/Resources/Setup.png",
                    Command = new DelegateCommand<object>(
                        o => ChangeScreenPage(RegionNames.PrimaryPageRegion, PageNames.ConfigPage),
                        o => _currentScreenPage != PageNames.ConfigPage )
                },

                new ToolBarItemModel()
                {
                    Text = "Roster",
                    Icon = "/LaJust.PowerMeter.Modules.Screens;component/Resources/User-Male.png",
                    Command = new DelegateCommand<object>(
                        o => ChangeScreenPage(RegionNames.PrimaryPageRegion, PageNames.RosterPage),
                        o => _currentScreenPage != PageNames.RosterPage )
                },

                new ToolBarItemModel()
                {
                    Text = "History",
                    Icon = "/LaJust.PowerMeter.Modules.Screens;component/Resources/Chart.png",
                    Command = new DelegateCommand<object>(
                        o => ChangeScreenPage(RegionNames.PrimaryPageRegion, PageNames.HistoryPage),
                        o => _currentScreenPage != PageNames.HistoryPage )
                }
            };

            // Register the toolbar items with the toolbar region
            _toolbarItems.ForEach(item => _regionManager.RegisterViewWithRegion(RegionNames.ToolbarRightRegion, () => item));
        }

        /// <summary>
        /// Called when [change screen page event].
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        private void ChangeScreenPage(string regionName, PageNames pageName)
        {
            // Check the region exists
            if (_regionManager.Regions.ContainsRegionWithName(regionName))
            {
                using (new ShowBusyIndicator())
                {
                    // Find an activate the page with the given name
                    IRegion region = _regionManager.Regions[regionName];
                    region.Activate(region.Views.First(x => ((FrameworkElement)x).Name == pageName.ToString()));

                    //TODO: This is broken since it doesn't account for the second screen page changes
                    _currentScreenPage = pageName;

                    // RaiseCanExecuteChanged method on each toolbar command
                    _toolbarItems.ForEach(item => ((DelegateCommand<object>)item.Command).RaiseCanExecuteChanged());

                    // Publish notification event
                    _eventAggregator.GetEvent<ScreenEvents.ActiveScreenChanged>().Publish(new ScreenEvents.ActiveScreenChanged()
                    {
                        RegionName = regionName,
                        PageName = pageName
                    });
                }
            }
        }

    }
}
