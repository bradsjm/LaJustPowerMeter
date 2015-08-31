// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphingEvents.cs" company="">
//   
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// <summary>
//   Game Events
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using Microsoft.Practices.Prism.Events;

    /// <summary>
    /// Game Events
    /// </summary>
    public static class GraphingEvents
    {
        /// <summary>
        /// Raised when the score changes
        /// </summary>
        public class ShowPopupGraph : CompositePresentationEvent<ShowPopupGraph>
        {
            #region Properties

            /// <summary>
            /// Gets or sets the sensor id.
            /// </summary>
            /// <value>The sensor id.</value>
            public string SensorId { get; set; }

            #endregion
        }
    }
}