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

namespace LaJust.PowerMeter.Modules.CountDownClock.UserControls
{
    /// <summary>
    /// Interaction logic for ClockDigits.xaml
    /// </summary>
    public partial class ClockDigits : UserControl
    {
        #region Private Fields

        private const string DIGIT_URI = "/LaJust.PowerMeter.Modules.CountDownClock;component/Resources/ClockDigits/{0}.png";

        /// <summary>
        /// Array of bitmap images for the digits 0 through 9
        /// </summary>
        private static BitmapImage[] _digitImages;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="ClockDigits"/> class.
        /// </summary>
        static ClockDigits()
        {
            _digitImages = new BitmapImage[10];
            for (int i = 0; i < _digitImages.Length; i++)
            {
                _digitImages[i] = new BitmapImage(new Uri(string.Format(DIGIT_URI, i), UriKind.Relative));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockDigits"/> class.
        /// </summary>
        public ClockDigits()
        {
            InitializeComponent();

            // Enable animations after initial loading has completed
            Dispatcher.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                MinuteDigit1Box.TransitionsEnabled = true;
                MinuteDigit2Box.TransitionsEnabled = true;
                SecondsDigit1Box.TransitionsEnabled = true;
                SecondsDigit2Box.TransitionsEnabled = true;
            }, System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value indicating whether [show tenths].
        /// </summary>
        /// <value><c>true</c> if [show tenths]; otherwise, <c>false</c>.</value>
        public bool ShowTenths
        {
            get { return (bool)GetValue(ShowTenthsProperty); }
            set { SetValue(ShowTenthsProperty, value); }
        }

        public static readonly DependencyProperty ShowTenthsProperty =
           DependencyProperty.Register(
              "ShowTenths",
              typeof(bool),
              typeof(ClockDigits),
              new FrameworkPropertyMetadata(true, new PropertyChangedCallback(ChangeShowTenthsProperty)));

        private static void ChangeShowTenthsProperty(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as ClockDigits).HundrethsDigit.Visibility = ((bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed);
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

        public static readonly DependencyProperty EditableProperty =
           DependencyProperty.Register(
              "Editable",
              typeof(bool),
              typeof(ClockDigits),
              new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The score.</value>
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
           DependencyProperty.Register(
              "Time",
              typeof(TimeSpan),
              typeof(ClockDigits),
              new FrameworkPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(ChangeTime)));

        /// <summary>
        /// Changes the time.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeTime(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as ClockDigits).UpdateTime((TimeSpan)e.NewValue);
        }

        /// <summary>
        /// Updates the time.
        /// </summary>
        /// <param name="NewTime">The new time.</param>
        private void UpdateTime(TimeSpan NewTime)
        {
            if (NewTime >= TimeSpan.Zero)
            {
                int minutes1 = NewTime.Minutes / 10;
                int minutes2 = NewTime.Minutes % 10;
                int seconds1 = NewTime.Seconds / 10;
                int seconds2 = NewTime.Seconds % 10;
                int hundreths = NewTime.Milliseconds / 100;

                if (minutes1 == 0)
                {
                    MinuteDigit1Box.Tag = null;
                    MinuteDigit1Box.Content = null;
                }
                else if (MinuteDigit1Box.Tag != _digitImages[minutes1])
                {
                    MinuteDigit1Box.Tag = _digitImages[minutes1];
                    MinuteDigit1Box.Content = new Image() { Source = _digitImages[minutes1], Stretch = Stretch.Uniform };
                }

                if (MinuteDigit2Box.Tag != _digitImages[minutes2])
                {
                    MinuteDigit2Box.Tag = _digitImages[minutes2];
                    MinuteDigit2Box.Content = new Image() { Source = _digitImages[minutes2], Stretch = Stretch.Uniform };
                }

                if (SecondsDigit1Box.Tag != _digitImages[seconds1])
                {
                    SecondsDigit1Box.Tag = _digitImages[seconds1];
                    SecondsDigit1Box.Content = new Image() { Source = _digitImages[seconds1], Stretch = Stretch.Uniform };
                }

                if (SecondsDigit2Box.Tag != _digitImages[seconds2])
                {
                    SecondsDigit2Box.Tag = _digitImages[seconds2];
                    SecondsDigit2Box.Content = new Image() { Source = _digitImages[seconds2], Stretch = Stretch.Uniform };
                }

                if (HundrethsDigit.Source != _digitImages[hundreths])
                    HundrethsDigit.Source = _digitImages[hundreths];
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
                int newValue = (Time.Minutes / 10 < 5 ? Time.Minutes + 10 : Time.Minutes - 50);
                this.Time = new TimeSpan(Time.Days, Time.Hours, newValue, Time.Seconds, 0);
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
                int newValue = (Time.Minutes % 10 < 9 ? Time.Minutes + 1 : Time.Minutes - 9);
                this.Time = new TimeSpan(Time.Days, Time.Hours, newValue, Time.Seconds, 0);
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
                int newValue = (Time.Seconds / 10 < 5 ? Time.Seconds + 10 : Time.Seconds - 50);
                this.Time = new TimeSpan(Time.Days, Time.Hours, Time.Minutes, newValue, 0);
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
                int newValue = (Time.Seconds % 10 < 9 ? Time.Seconds + 1 : Time.Seconds - 9);
                this.Time = new TimeSpan(Time.Days, Time.Hours, Time.Minutes, newValue, 0);
            }
        }

        #endregion
    }
}
