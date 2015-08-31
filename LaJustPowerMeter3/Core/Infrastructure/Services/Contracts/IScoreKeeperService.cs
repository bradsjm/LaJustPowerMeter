// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScoreKeeperService.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Score Keeper Service Interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System.ComponentModel;

    /// <summary>
    /// Score Keeper Service Interface
    /// </summary>
    public interface IScoreKeeperService : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Gets the collection of competitors.
        /// </summary>
        /// <value>The competitors.</value>
        DispatchingObservableCollection<CompetitorModel> Competitors { get; }

        /// <summary>
        /// Gets or sets the game number.
        /// </summary>
        /// <value>The game number.</value>
        byte GameNumber { get; set; }

        /// <summary>
        /// Gets the current round number.
        /// </summary>
        /// <value>The round number.</value>
        byte RoundNumber { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Persists the state.
        /// </summary>
        void PersistCompetitorState();

        /// <summary>
        /// Resets the scores.
        /// </summary>
        void ResetScores();

        /// <summary>
        /// Restores the state.
        /// </summary>
        void RestoreCompetitorState();

        #endregion
    }
}