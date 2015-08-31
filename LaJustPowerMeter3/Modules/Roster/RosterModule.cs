// <copyright file="RosterModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Roster
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Infrastructure;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Prism.Events;
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Chronograph Module providing clock and stopwatch
    /// </summary>
    [ModuleExport(typeof(RosterModule))]
    public sealed class RosterModule : IModule
    {
        private readonly CompositionContainer container;

        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEngineModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public RosterModule(CompositionContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.regionManager.AddToRegion(
                RegionNames.ToolbarRightRegion,
                new ToolBarItemModel()
                {
                    Text = "Roster",
                    Icon = "/Roster;component/Resources/User-Male.png",
                    Command = new DelegateCommand(delegate
                    {
                        var page = this.regionManager.Regions[RegionNames.PrimaryPageRegion].GetView(PageNames.Roster);
                        this.regionManager.Regions[RegionNames.PrimaryPageRegion].Activate(page);
                    })
                });
        }
    }
}
