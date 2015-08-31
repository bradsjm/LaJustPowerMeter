namespace LaJust.PowerMeter.Common.BaseClasses
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Reflection;
    using LaJust.PowerMeter.Common.BaseClasses;

    public class ConfigBase : PropertyNotifier
    {
        /// <summary>
        /// Populates the specified config type from the settings collection.
        /// </summary>
        /// <param name="configType">Type of the config.</param>
        /// <param name="settings">The settings.</param>
        public void Populate(SettingsPropertyValueCollection settings)
        {
            foreach (PropertyInfo configProperty in this.GetType().GetProperties())
            {
                configProperty.SetValue(this, settings[configProperty.Name].PropertyValue, null);
            }
        }

        /// <summary>
        /// Updates the specified settings from this config class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void Update(SettingsPropertyValueCollection settings)
        {
            foreach (PropertyInfo configProperty in this.GetType().GetProperties())
            {
                settings[configProperty.Name].PropertyValue = configProperty.GetValue(this, null);
            }
        }

    }
}
