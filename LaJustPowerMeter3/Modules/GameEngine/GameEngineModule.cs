// <copyright file="GameEngineModule.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Infrastructure;
    using Microsoft.Practices.Prism.MefExtensions.Modularity;
    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Chronograph Module providing clock and stopwatch
    /// </summary>
    [ModuleExport(typeof(GameEngineModule))]
    public sealed class GameEngineModule : IModule
    {
        private readonly CompositionContainer container;

        private readonly IRegionManager regionManager;

        private GameBase Game;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEngineModule"/> class.
        /// </summary>
        [ImportingConstructor]
        public GameEngineModule(CompositionContainer container, IRegionManager regionManager)
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
                    Text = "Event",
                    Icon = "/GameEngine;component/Resources/Clock.png",
                    Command = new DelegateCommand(delegate
                    {
                        var page = this.regionManager.Regions[RegionNames.PrimaryPageRegion].GetView(PageNames.Event);
                        this.regionManager.Regions[RegionNames.PrimaryPageRegion].Activate(page);
                    })
                });

            this.Game = container.GetExportedValue<KickAndPunch>();
            this.Game.Run();
        }
    }
}
