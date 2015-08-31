// <copyright file="StrikePanelDisplay.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ImpactMeter
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
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for StrikePanelDisplay.xaml
    /// </summary>
    public partial class StrikePanelDisplay : UserControl
    {
        /// <summary>
        /// Start Panel Number Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartPanelNumProperty =
           DependencyProperty.Register(
              "StartPanelNum",
              typeof(int),
              typeof(StrikePanelDisplay),
              new PropertyMetadata((int)0));

        /// <summary>
        /// Panel Number Dependency Property
        /// </summary>
        public static readonly DependencyProperty PanelNumProperty =
           DependencyProperty.Register(
              "PanelNum",
              typeof(int),
              typeof(StrikePanelDisplay),
              new FrameworkPropertyMetadata((int)0, new PropertyChangedCallback(ChangePanelNum)));

        /// <summary>
        /// Initializes a new instance of the <see cref="StrikePanelDisplay"/> class.
        /// </summary>
        public StrikePanelDisplay()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the StartPanelNum.
        /// </summary>
        /// <value>The PanelNum.</value>
        public int StartPanelNum
        {
            get { return (int)GetValue(StartPanelNumProperty); }
            set { SetValue(StartPanelNumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the PanelNum.
        /// </summary>
        /// <value>The PanelNum.</value>
        public int PanelNum
        {
            get { return (int)GetValue(PanelNumProperty); }
            set { SetValue(PanelNumProperty, value); }
        }

        /// <summary>
        /// Changes the PanelNum.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangePanelNum(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as StrikePanelDisplay).UpdatePanelNum((int)e.OldValue, (int)e.NewValue);
            }
        }

        /// <summary>
        /// Updates the PanelNum.
        /// </summary>
        /// <param name="oldPanelNum">The old panel num.</param>
        /// <param name="newPanelNum">The new panel num.</param>
        private void UpdatePanelNum(int oldPanelNum, int newPanelNum)
        {
            int panel = newPanelNum - this.StartPanelNum;

            if (panel >= 0 && panel < this.Panels.Children.Count)
            {
                ((Rectangle)Panels.Children[panel]).Fill = Brushes.DarkOrange;
            }

            panel = oldPanelNum - this.StartPanelNum;
            if (panel >= 0 && panel < this.Panels.Children.Count)
            {
                ((Rectangle)Panels.Children[panel]).Fill = Brushes.Transparent;
            }
        }
    }
}
