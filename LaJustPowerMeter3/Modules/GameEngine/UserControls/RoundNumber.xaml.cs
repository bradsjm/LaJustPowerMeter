// <copyright file="RoundNumber.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace GameEngine
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
    
    /// <summary>
    /// Interaction logic for RoundNumber.xaml
    /// </summary>
    public partial class RoundNumber : UserControl
    {        
        /// <summary>
        /// Round Number Dependency Property
        /// </summary>
        public static readonly DependencyProperty RoundProperty =
           DependencyProperty.Register(
              "Round",
              typeof(int),
              typeof(RoundNumber),
              new FrameworkPropertyMetadata((int)0, new PropertyChangedCallback(ChangeRound)));

        /// <summary>
        /// The location of the round number images
        /// </summary>
        private const string ROUNDURI = "/GameEngine;component/Resources/Rounds/{0}R.png";

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundNumber"/> class.
        /// </summary>
        public RoundNumber()
        {
            this.InitializeComponent();

            // Enable animations after initial loading has completed
            Dispatcher.BeginInvoke(
                (Action)delegate
                {
                    this.RoundNumberBox.TransitionsEnabled = true;
                },
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public int Round
        {
            get { return (int)GetValue(RoundProperty); }
            set { SetValue(RoundProperty, value); }
        }

        /// <summary>
        /// Changes the round number.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeRound(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as RoundNumber).UpdateRound((int)e.NewValue);
            }
        }

        /// <summary>
        /// Updates the round number.
        /// </summary>
        /// <param name="newRoundNumber">The new round number.</param>
        private void UpdateRound(int newRoundNumber)
        {
            this.RoundNumberBox.Content = new Image() 
            { 
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(new Uri(string.Format(ROUNDURI, newRoundNumber), UriKind.Relative))
            };
        }
    }
}
