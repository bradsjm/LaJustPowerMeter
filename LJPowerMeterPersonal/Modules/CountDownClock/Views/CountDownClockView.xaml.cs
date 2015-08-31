/*
 * © Copyright 2009 Jonathan Bradshaw, LaJust Sports America, Inc. All Rights Reserved. 
 */
namespace LaJust.PowerMeter.Modules.CountDownClock.Views
{
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
    using LaJust.PowerMeter.Modules.CountDownClock.Presenters;
    using LaJust.PowerMeter.Common.BaseClasses;

    /// <summary>
    /// Interaction logic for CountDownClockView.xaml
    /// </summary>
    public partial class CountDownClockView : UserControl, IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountDownClockView"/> class.
        /// </summary>
        public CountDownClockView()
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
