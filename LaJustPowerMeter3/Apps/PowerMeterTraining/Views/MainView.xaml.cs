// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainView.xaml.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Shell
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    [Export(typeof(MainView))]
    public partial class MainView : Window
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
            this.Title += " " + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", 
            Justification = "Needs to be a property to be composed by MEF")]
        private MainViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Exit Button control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "Are you sure you want to exit?", 
                    "Confirm Application Exit", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question, 
                    MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}