// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigService.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// The app configuration service.
    /// </summary>
    [Export(typeof(IAppConfigService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AppConfigService : IAppConfigService
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigService"/> class.
        /// </summary>
        /// <param name="appClosingEvent">
        /// The app Closing Event.
        /// </param>
        [ImportingConstructor]
        public AppConfigService(ApplicationEvents.ApplicationClosing appClosingEvent)
        {
            this.Settings = new AppSettingsModel();

            // this.Load();
            // appClosingEvent.Subscribe(e => this.Save(), true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the application configuration settings.
        /// </summary>
        /// <value>The settings.</value>
        public AppSettingsModel Settings { get; private set; }

        #endregion
    }
}