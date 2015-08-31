// <copyright file="EventScreenViewModel.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
{
    using System.ComponentModel.Composition;
    using Microsoft.Practices.Prism.ViewModel;
    using Infrastructure;

    /// <summary>
    /// View Model for Main Window View
    /// </summary>
    [Export]
    public class EventScreenViewModel : NotificationObject
    {
        /// <summary>
        /// The Score Keeper Service Reference
        /// </summary>
        private readonly IScoreKeeperService ScoreKeeperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDataViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="scoreService">The score service.</param>
        [ImportingConstructor]
        public EventScreenViewModel(IScoreKeeperService scoreService)
        {
            this.ScoreKeeperService = scoreService;
        }

        /// <summary>
        /// Gets the game meta data model.
        /// </summary>
        /// <value>The game meta data model.</value>
        public IScoreKeeperService ScoreKeeper
        {
            get { return this.ScoreKeeperService; }
        }
    }
}
