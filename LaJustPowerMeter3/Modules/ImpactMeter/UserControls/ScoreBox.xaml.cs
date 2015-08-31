// <copyright file="ScoreBox.xaml.cs" company="LaJust Sports America">
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

    /// <summary>
    /// Interaction logic for ScoreBox.xaml
    /// </summary>
    public partial class ScoreBox : UserControl
    {
        /// <summary>
        /// Score Dependency Property
        /// </summary>
        public static readonly DependencyProperty ScoreProperty =
           DependencyProperty.Register(
              "Score",
              typeof(uint),
              typeof(ScoreBox),
              new FrameworkPropertyMetadata((uint)0, new PropertyChangedCallback(ChangeScore)));

        /// <summary>
        /// Location of the digit images
        /// </summary>
        private const string DIGITURI = "/ImpactMeter;component/Resources/ScoreDigits/{0}.png";

        /// <summary>
        /// Digit Images Array
        /// </summary>
        private static BitmapImage[] digitImages;

        /// <summary>
        /// Initializes static members of the <see cref="ScoreBox"/> class.
        /// </summary>
        static ScoreBox()
        {
            digitImages = new BitmapImage[10];
            for (int i = 0; i < digitImages.Length; i++)
            {
                digitImages[i] = new BitmapImage(new Uri(string.Format(DIGITURI, i), UriKind.Relative));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreBox"/> class.
        /// </summary>
        public ScoreBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public uint Score
        {
            get { return (uint)GetValue(ScoreProperty); }
            set { SetValue(ScoreProperty, value); }
        }

        /// <summary>
        /// Changes the score.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeScore(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as ScoreBox).UpdateScore((uint)e.NewValue);
            }
        }

        /// <summary>
        /// Updates the score.
        /// </summary>
        /// <param name="newScore">The new score.</param>
        private void UpdateScore(uint newScore)
        {
            newScore = Math.Min(newScore, 999);

            uint hundreds = newScore / 100;
            uint tens = (newScore - ((newScore / 100) * 100)) / 10;
            uint ones = newScore % 10;

            if (this.Hundreds.Source != digitImages[hundreds])
            {
                Hundreds.Source = digitImages[hundreds];
            }

            if (this.Tens.Source != digitImages[tens])
            {
                Tens.Source = digitImages[tens];
            }

            if (this.Ones.Source != digitImages[ones])
            {
                Ones.Source = digitImages[ones];
            }

            this.BlinkScorebox();
        }

        /// <summary>
        /// Blinks the scorebox.
        /// </summary>
        private void BlinkScorebox()
        {
            ((Storyboard)FindResource("Blink")).Begin();
        }
    }
}
