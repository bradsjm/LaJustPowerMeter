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

namespace LaJust.PowerMeter.Modules.GameEngine.UserControls
{
    /// <summary>
    /// Interaction logic for RoundNumber.xaml
    /// </summary>
    public partial class RoundNumber : UserControl
    {        
        #region Private Fields

        private const string ROUND_URI = "/LaJust.PowerMeter.Modules.GameEngine;component/Resources/Rounds/{0}R.png";

        #endregion

        public RoundNumber()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the round number.
        /// </summary>
        /// <value>The round number.</value>
        public int Round
        {
            get { return (int)GetValue(RoundProperty); }
            set { SetValue(RoundProperty, value); }
        }

        public static readonly DependencyProperty RoundProperty =
           DependencyProperty.Register(
              "Round",
              typeof(int),
              typeof(RoundNumber),
              new FrameworkPropertyMetadata((int)0, new PropertyChangedCallback(ChangeRound)));

        /// <summary>
        /// Changes the round number.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeRound(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as RoundNumber).UpdateRound((int)e.NewValue);
        }

        /// <summary>
        /// Updates the round number.
        /// </summary>
        /// <param name="newRoundNumber">The new round number.</param>
        private void UpdateRound(int newRoundNumber)
        {
            RoundNumberBox.Content = new Image() 
                { 
                    Stretch = Stretch.Uniform,
                    Source = new BitmapImage(new Uri(string.Format(ROUND_URI, newRoundNumber), UriKind.Relative))
                };
        }

        #endregion

    }
}
