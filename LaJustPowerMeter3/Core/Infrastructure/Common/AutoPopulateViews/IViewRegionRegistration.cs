// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewRegionRegistration.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    /// <summary>
    /// View Region Registration Interface
    /// </summary>
    public interface IViewRegionRegistration
    {
        #region Properties

        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        /// <value>The name of the region.</value>
        string RegionName { get; }

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <value>The name of the view.</value>
        string ViewName { get; }

        #endregion
    }
}