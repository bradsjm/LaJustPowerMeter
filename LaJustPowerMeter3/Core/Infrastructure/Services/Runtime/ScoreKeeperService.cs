// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScoreKeeperService.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Logging;
    using Microsoft.Practices.Prism.ViewModel;

    /// <summary>
    /// Stop Watch Service
    /// </summary>
    [Export(typeof(IScoreKeeperService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ScoreKeeperService : NotificationObject, IScoreKeeperService
    {
        #region Constants and Fields

        /// <summary>
        /// Composite Application Library Logger
        /// </summary>
        protected readonly ILoggerFacade Logger;

        /// <summary>
        /// The state configuration file name.
        /// </summary>
        private const string CompetitorStateFileName = "CompetitorState.xml";

        /// <summary>
        /// Current Game Number
        /// </summary>
        private byte gameNumber;

        /// <summary>
        /// Current Round Number for the game
        /// </summary>
        private byte roundNumber;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreKeeperService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        [ImportingConstructor]
        public ScoreKeeperService(ILoggerFacade logger)
        {
            this.Logger = logger;
            this.Competitors = new DispatchingObservableCollection<CompetitorModel>();
            logger.Log("ScoreKeeperService Initialized", Category.Debug, Priority.None);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an Observable collection of competitors
        /// </summary>
        public DispatchingObservableCollection<CompetitorModel> Competitors { get; private set; }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        public byte GameNumber
        {
            get
            {
                return this.gameNumber;
            }

            set
            {
                this.gameNumber = value;
                this.RoundNumber = 1;
                this.RaisePropertyChanged(() => this.GameNumber);
                this.Logger.Log("ScoreKeeperService: Game Number set to " + value, Category.Debug, Priority.None);
            }
        }

        /// <summary>
        /// Gets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public byte RoundNumber
        {
            get
            {
                return this.roundNumber;
            }

            private set
            {
                this.roundNumber = value;
                this.RaisePropertyChanged(() => this.RoundNumber);
                this.Logger.Log("ScoreKeeperService: Round Number set to " + value, Category.Debug, Priority.None);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IScoreKeeperService

        /// <summary>
        /// Persists the state of the competitors.
        /// </summary>
        public void PersistCompetitorState()
        {
            var xs = new XmlSerializer(typeof(Collection<CompetitorModel>));
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                Solution.Company, 
                Solution.Product);
            string file = Path.Combine(path, CompetitorStateFileName);
            Directory.CreateDirectory(path);

            using (var sw = new StreamWriter(file, false))
            {
                xs.Serialize(sw, this.Competitors);
            }

            ;

            this.Logger.Log("ScoreKeeperService: Persisted State to " + file, Category.Debug, Priority.None);
        }

        /// <summary>
        /// Resets the scores.
        /// </summary>
        public void ResetScores()
        {
            foreach (var competitor in this.Competitors)
            {
                competitor.Reset();
            }

            this.Logger.Log("ScoreKeeperService: All Scores Reset", Category.Debug, Priority.None);
        }

        /// <summary>
        /// Loads the state of the competitors.
        /// </summary>
        public void RestoreCompetitorState()
        {
            var xs = new XmlSerializer(typeof(DispatchingObservableCollection<CompetitorModel>));
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                Solution.Company, 
                Solution.Product);
            var file = Path.Combine(path, CompetitorStateFileName);

            if (File.Exists(file))
            {
                using (var sr = new StreamReader(file))
                {
                    var competitors = xs.Deserialize(sr) as Collection<CompetitorModel>;
                    this.Competitors.AddRange(competitors);
                }

                this.Logger.Log(
                    "ScoreKeeperService: Restored Competitor State from " + file, Category.Debug, Priority.None);
            }
        }

        #endregion

        #endregion
    }
}