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
using System.Windows.Media.Animation;
namespace LaJust.PowerMeter.Modules.Meter.UserControls
{
    /// <summary>
    /// Interaction logic for ScoreBox.xaml
    /// </summary>
    public partial class ScoreBox : UserControl
    {
        private const string DIGIT_URI = "/LaJust.PowerMeter.Modules.Meter;component/Resources/ScoreDigits/{0}.png";
        private static BitmapImage[] _digitImages;

        static ScoreBox()
        {
            _digitImages = new BitmapImage[10];
            for (int i = 0; i < _digitImages.Length; i++)
            {
                _digitImages[i] = new BitmapImage(new Uri(string.Format(DIGIT_URI, i), UriKind.Relative));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterView"/> class.
        /// </summary>
        public ScoreBox()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public uint Score
        {
            get { return (uint)GetValue(ScoreProperty); }
            set { SetValue(ScoreProperty, value); }
        }

        public static readonly DependencyProperty ScoreProperty =
           DependencyProperty.Register(
              "Score",
              typeof(uint),
              typeof(ScoreBox),
              new FrameworkPropertyMetadata((uint)0, new PropertyChangedCallback(ChangeScore)));

        /// <summary>
        /// Changes the score.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeScore(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as ScoreBox).UpdateScore((uint)e.NewValue);
        }

        /// <summary>
        /// Updates the score.
        /// </summary>
        /// <param name="NewScore">The new score.</param>
        private void UpdateScore(uint NewScore)
        {
            NewScore = Math.Min(NewScore, 999);

            uint hundreds = NewScore / 100;
            uint tens = (NewScore - ((NewScore / 100) * 100)) / 10;
            uint ones = NewScore % 10;

            if (Hundreds.Source != _digitImages[hundreds])
                Hundreds.Source = _digitImages[hundreds];

            if (Tens.Source != _digitImages[tens])
                Tens.Source = _digitImages[tens];

            if (Ones.Source != _digitImages[ones])
                Ones.Source = _digitImages[ones];

            BlinkScorebox();
        }

        #endregion

        /// <summary>
        /// Blinks the scorebox.
        /// </summary>
        private void BlinkScorebox()
        {
            ((Storyboard)FindResource("Blink")).Begin();
        }

    }
}
