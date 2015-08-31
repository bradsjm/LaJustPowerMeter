// <copyright file="MeterColorSelector.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ImpactMeter
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Infrastructure;
    using LaJust.EIDSS.Communications.Entities;

    /// <summary>
    /// Returns color to use for meter
    /// </summary>
    public class MeterColorSelector : IValueConverter
    {
        /// <summary>
        /// Colors to be used for Targets
        /// </summary>
        private static string[] targetColors = new string[]
        {
            "#FFFFBA00",
            "#FFE31939",
            "#FFFF8B00",
            "#FF641CA2",
            "#FF00CF60",
            "#FF1E549E",
            "#FFC6D800",
            "#FFFF5F00"
        };

        /// <summary>
        /// Incrementing couter used to cycle through target colors
        /// </summary>
        private static byte targetCounter = 0;

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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is OpCodes)
            {
                OpCodes opCode = (OpCodes)value;

                switch (opCode)
                {
                    case OpCodes.ChungRegistered:
                        return "#FF007DC3"; // LaJust Blue

                    case OpCodes.HongRegistered:
                        return "#FFE31937"; // LaJust Red

                    case OpCodes.TargetRegistered:
                        return targetColors[(targetCounter++ % targetColors.Length)];
                }
            }

            return "Gray";
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
