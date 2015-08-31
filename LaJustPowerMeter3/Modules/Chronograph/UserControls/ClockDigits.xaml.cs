// <copyright file="ClockDigits.xaml.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace Chronograph
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
    /// Interaction logic for ClockDigits.xaml
    /// </summary>
    public partial class ClockDigits : UserControl
    {
        #region Dependency Property Fields

        /// <summary>
        /// Show the Tenths digit dependency property
        /// </summary>
        public static readonly DependencyProperty ShowTenthsProperty =
           DependencyProperty.Register(
              "ShowTenths",
              typeof(bool),
              typeof(ClockDigits),
              new FrameworkPropertyMetadata(true, new PropertyChangedCallback(ChangeShowTenthsProperty)));

        /// <summary>
        /// The Editable Dependency Property
        /// </summary>
        public static readonly DependencyProperty EditableProperty =
           DependencyProperty.Register(
              "Editable",
              typeof(bool),
              typeof(ClockDigits),
              new PropertyMetadata(false));

        /// <summary>
        /// The Time Dependency Property
        /// </summary>
        public static readonly DependencyProperty TimeProperty =
           DependencyProperty.Register(
              "Time",
              typeof(TimeSpan),
              typeof(ClockDigits),
              new FrameworkPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(ChangeTime)));

        #endregion

        #region Private Fields

        /// <summary>
        /// The location of the digit image resources
        /// </summary>
        private const string DIGITURI = "/Chronograph;component/Resources/ClockDigits/{0}.png";

        /// <summary>
        /// Array of bitmap images for the digits 0 through 9
        /// </summary>
        private static BitmapImage[] digitImages;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ClockDigits"/> class.
        /// </summary>
        static ClockDigits()
        {
            digitImages = new BitmapImage[10];
            for (int i = 0; i < digitImages.Length; i++)
            {
                digitImages[i] = new BitmapImage(new Uri(string.Format(DIGITURI, i), UriKind.Relative));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockDigits"/> class.
        /// </summary>
        public ClockDigits()
        {
            this.InitializeComponent();

            // Enable animations after initial loading has completed
            Dispatcher.BeginInvoke(
                (Action)delegate
                {
                    this.MinuteDigit1Box.TransitionsEnabled = true;
                    this.MinuteDigit2Box.TransitionsEnabled = true;
                    this.SecondsDigit1Box.TransitionsEnabled = true;
                    this.SecondsDigit2Box.TransitionsEnabled = true;
                },
                System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [show tenths].
        /// </summary>
        /// <value><c>true</c> if [show tenths]; otherwise, <c>false</c>.</value>
        public bool ShowTenths
        {
            get { return (bool)GetValue(ShowTenthsProperty); }
            set { SetValue(ShowTenthsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ClockDigits"/> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The score.</value>
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        #endregion

        #region Dependency Property Handlers

        /// <summary>
        /// Changes the show tenths property.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeShowTenthsProperty(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as ClockDigits).HundrethsDigit.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Changes the time.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeTime(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as ClockDigits).UpdateTime((TimeSpan)e.NewValue);
            }
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <param name="newTime">The new time.</param>
        private void UpdateTime(TimeSpan newTime)
        {
            if (newTime >= TimeSpan.Zero)
            {
                int minutes1 = newTime.Minutes / 10;
                int minutes2 = newTime.Minutes % 10;
                int seconds1 = newTime.Seconds / 10;
                int seconds2 = newTime.Seconds % 10;
                int hundreths = newTime.Milliseconds / 100;

                if (minutes1 == 0)
                {
                    this.MinuteDigit1Box.Tag = null;
                    this.MinuteDigit1Box.Content = null;
                }
                else if (this.MinuteDigit1Box.Tag != digitImages[minutes1])
                {
                    this.MinuteDigit1Box.Tag = digitImages[minutes1];
                    this.MinuteDigit1Box.Content = new Image() { Source = digitImages[minutes1], Stretch = Stretch.Uniform };
                }

                if (this.MinuteDigit2Box.Tag != digitImages[minutes2])
                {
                    this.MinuteDigit2Box.Tag = digitImages[minutes2];
                    this.MinuteDigit2Box.Content = new Image() { Source = digitImages[minutes2], Stretch = Stretch.Uniform };
                }

                if (this.SecondsDigit1Box.Tag != digitImages[seconds1])
                {
                    this.SecondsDigit1Box.Tag = digitImages[seconds1];
                    this.SecondsDigit1Box.Content = new Image() { Source = digitImages[seconds1], Stretch = Stretch.Uniform };
                }

                if (this.SecondsDigit2Box.Tag != digitImages[seconds2])
                {
                    this.SecondsDigit2Box.Tag = digitImages[seconds2];
                    this.SecondsDigit2Box.Content = new Image() { Source = digitImages[seconds2], Stretch = Stretch.Uniform };
                }

                if (this.HundrethsDigit.Source != digitImages[hundreths])
                {
                    this.HundrethsDigit.Source = digitImages[hundreths];
                }
            }
        }

        #endregion

        #region Mouse Click Handlers

        /// <summary>
        /// Handles the MouseUp event of the MinuteDigit1Box control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MinuteDigit1Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Editable)
            {
                int newValue = this.Time.Minutes / 10 < 5 ? this.Time.Minutes + 10 : this.Time.Minutes - 50;
                this.Time = new TimeSpan(this.Time.Days, this.Time.Hours, newValue, this.Time.Seconds, 0);
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the MinuteDigit2Box control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MinuteDigit2Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Editable)
            {
                int newValue = this.Time.Minutes % 10 < 9 ? this.Time.Minutes + 1 : this.Time.Minutes - 9;
                this.Time = new TimeSpan(this.Time.Days, this.Time.Hours, newValue, this.Time.Seconds, 0);
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the SecondsDigit1Box control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SecondsDigit1Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Editable)
            {
                int newValue = this.Time.Seconds / 10 < 5 ? this.Time.Seconds + 10 : this.Time.Seconds - 50;
                this.Time = new TimeSpan(this.Time.Days, this.Time.Hours, this.Time.Minutes, newValue, 0);
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the SecondsDigit2Box control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SecondsDigit2Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Editable)
            {
                int newValue = this.Time.Seconds % 10 < 9 ? this.Time.Seconds + 1 : this.Time.Seconds - 9;
                this.Time = new TimeSpan(this.Time.Days, this.Time.Hours, this.Time.Minutes, newValue, 0);
            }
        }
        #endregion
    }
}
