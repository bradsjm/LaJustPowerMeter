namespace LaJust.PowerMeter.Common.Models
{
    using System.Windows.Input;

    /// <summary>
    /// Provides a common model used for populating the tool bar view
    /// </summary>
    public class ToolBarItemModel
    {
        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public string Icon { get; set; }
    }
}
