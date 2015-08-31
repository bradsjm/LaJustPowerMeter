namespace Infrastructure
{
    using System;

    public interface IAppConfigService
    {
        /// <summary>
        /// Loads the configuration from disk.
        /// </summary>
        //void Load();

        /// <summary>
        /// Saves the configuration to disk.
        /// </summary>
        //void Save();

        /// <summary>
        /// Gets the application configuration settings.
        /// </summary>
        /// <value>The settings.</value>
        AppSettingsModel Settings { get; }
    }
}
