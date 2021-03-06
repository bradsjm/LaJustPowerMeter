﻿using System;
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
using LaJust.PowerMeter.Modules.Receiver.Presenters;
using LaJust.PowerMeter.Common.BaseClasses;

namespace LaJust.PowerMeter.Modules.Receiver.Views
{
    /// <summary>
    /// Interaction logic for ReceiverStateView.xaml
    /// </summary>
    public partial class ReceiverStateView : UserControl, IView
    {
        public ReceiverStateView()
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
