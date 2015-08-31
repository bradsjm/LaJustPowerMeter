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
using LaJust.PowerMeter.Modules.Meter.Presenters;
using LaJust.PowerMeter.Common.BaseClasses;

namespace LaJust.PowerMeter.Modules.Meter.Views
{
    /// <summary>
    /// Interaction logic for MeterView.xaml
    /// </summary>
    public partial class MeterView : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeterView"/> class.
        /// </summary>
        public MeterView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
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
