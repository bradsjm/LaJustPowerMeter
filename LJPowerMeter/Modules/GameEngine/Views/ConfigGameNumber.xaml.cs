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
using LaJust.PowerMeter.Common.BaseClasses;

namespace LaJust.PowerMeter.Modules.GameEngine.Views
{
    /// <summary>
    /// Interaction logic for ConfigGameNumber.xaml
    /// </summary>
    public partial class ConfigGameNumber : UserControl, IView
    {
        public ConfigGameNumber()
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
