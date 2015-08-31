using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Printing;
using LaJust.PowerMeter.Common.BaseClasses;

namespace LaJust.PowerMeter.Modules.DataGraph.Views
{
    /// <summary>
    /// Interaction logic for GameImpactPrintView.xaml
    /// </summary>
    public partial class GameImpactPrintView : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameImpactPrintView"/> class.
        /// </summary>
        public GameImpactPrintView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prints this instance.
        /// </summary>
        public bool Print()
        {
            try
            {
                PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
                bool? results = printDlg.ShowDialog();
                if (results == null || results == false) return false;

                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                //get the size of the printer page
                System.Windows.Size printSize = new System.Windows.Size(
                    capabilities.PageImageableArea.ExtentHeight, capabilities.PageImageableArea.ExtentWidth);

                // Build print view
                this.Height = printSize.Height;
                this.Width = printSize.Width;
                Measure(printSize);
                Arrange(new System.Windows.Rect(printSize));
                XpsDocumentWriter xpsdw = PrintQueue.CreateXpsDocumentWriter(printDlg.PrintQueue);
                printDlg.PrintTicket.PageOrientation = PageOrientation.Landscape;

                xpsdw.WriteAsync(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "Printing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        #region IView Members

        /// <summary>
        /// Applies the presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        public void ApplyPresenter(object presenter)
        {
            this.DataContext = presenter;
        }

        #endregion
    }
}
