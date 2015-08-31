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
using LaJust.PowerMeter.Modules.DataGraph.Presenters;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;
using System.Windows.Threading;
using LaJust.PowerMeter.Common.BaseClasses;
using Microsoft.Practices.Composite;

namespace LaJust.PowerMeter.Modules.DataGraph.Views
{
    /// <summary>
    /// Interaction logic for HistoryGraphView.xaml
    /// </summary>
    public partial class GameSelectorView : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarView"/> class.
        /// </summary>
        public GameSelectorView()
        {
            InitializeComponent();
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
