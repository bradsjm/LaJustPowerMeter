// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolBarItemModel.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using Microsoft.Practices.Prism.Commands;

    /// <summary>
    /// Provides a common model used for populating the tool bar view
    /// </summary>
    public class ToolBarItemModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public DelegateCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The button icon.</value>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The command parameter.</value>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>The button text.</value>
        public string Text { get; set; }

        #endregion
    }
}