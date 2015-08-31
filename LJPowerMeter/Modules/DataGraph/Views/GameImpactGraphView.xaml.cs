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
using LaJust.PowerMeter.Common.BaseClasses;

namespace LaJust.PowerMeter.Modules.DataGraph.Views
{
    /// <summary>
    /// Interaction logic for GameImpactGraphView.xaml
    /// </summary>
    public partial class GameImpactGraphView : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameImpactGraphView"/> class.
        /// </summary>
        public GameImpactGraphView()
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
