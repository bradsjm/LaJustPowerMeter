namespace LaJust.PowerMeter.Common.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Provides a helper converter to convert a boolean value into visibility values
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisible = (value != null) ? (bool)value : false;

            Visibility trueVisibility = Visibility.Visible;
            Visibility falseVisibility = Visibility.Collapsed;

            // We can override and specify the visibility using the parameter field
            // e.g. "Visible,Collapsed" maps to true and false
            if (parameter != null)
            {
                string[] parameters = parameter.ToString().Split('|');
                trueVisibility = (Visibility)Enum.Parse(typeof(Visibility), parameters[0], true);
                falseVisibility = (Visibility)Enum.Parse(typeof(Visibility), parameters[1], true);
            }

            return (isVisible ? trueVisibility : falseVisibility);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
