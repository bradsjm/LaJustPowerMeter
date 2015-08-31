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
using Microsoft.Practices.Composite;
using System.Windows.Media.Animation;

namespace LaJust.PowerMeter.Modules.DataGraph.Views
{
    /// <summary>
    /// Interaction logic for MeterGraphView.xaml
    /// </summary>
    public partial class MeterGraphPopupView : UserControl, IView, IActiveAware
    {
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterGraphPopupView"/> class.
        /// </summary>
        public MeterGraphPopupView()
        {
            InitializeComponent();
            IsActiveChanged += OnIsActiveChanged;
            this.Opacity = 0;
        }

        /// <summary>
        /// Handles the IsActiveChanged event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnIsActiveChanged(object sender, EventArgs e)
        {
            if (IsActive)
            {
                Dispatcher.BeginInvoke((Action)(delegate
                {
                    BeginAnimation(OpacityProperty, new DoubleAnimation()
                    {
                        From = 0,
                        To = 1.0,
                        Duration = new Duration(TimeSpan.FromMilliseconds(250))
                    });
                }), System.Windows.Threading.DispatcherPriority.ContextIdle);
            }
            else
            {
                Opacity = 0;
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

        #region IActiveAware Members

        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if the object is active; otherwise <see langword="false"/>.
        /// </value>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; IsActiveChanged(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Notifies that the value for <see cref="P:Microsoft.Practices.Composite.IActiveAware.IsActive"/> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged = delegate { };

        #endregion
    }
}
