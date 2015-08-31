// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisibilityConverter.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Provides a helper converter to convert a Boolean value into visibility values
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">
        /// The value produced by the binding source.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (value != null) ? (bool)value : false;

            var trueVisibility = Visibility.Visible;
            var falseVisibility = Visibility.Collapsed;

            // We can override and specify the visibility using the parameter field
            // e.g. "Visible,Collapsed" maps to true and false
            if (parameter != null)
            {
                var parameters = parameter.ToString().Split('|');
                trueVisibility = (Visibility)Enum.Parse(typeof(Visibility), parameters[0], true);
                falseVisibility = (Visibility)Enum.Parse(typeof(Visibility), parameters[1], true);
            }

            return isVisible ? trueVisibility : falseVisibility;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">
        /// The value that is produced by the binding target.
        /// </param>
        /// <param name="targetType">
        /// The type to convert to.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}