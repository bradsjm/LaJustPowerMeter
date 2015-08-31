// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalView.xaml.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System.Windows;
    using System.Windows.Forms;

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class ExternalView : Window
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalView"/> class.
        /// </summary>
        public ExternalView()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sizes to screen.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        public void SizeToScreen(Screen screen)
        {
            this.Left = screen.WorkingArea.Left;
            this.Top = screen.WorkingArea.Top;
            this.Width = screen.WorkingArea.Width;
            this.Height = screen.WorkingArea.Height;
        }

        #endregion
    }
}