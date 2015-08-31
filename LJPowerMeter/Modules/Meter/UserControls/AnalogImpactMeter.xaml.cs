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
using System.Windows.Threading;

namespace LaJust.PowerMeter.Modules.Meter.UserControls
{
    /// <summary>
    /// Interaction logic for AnalogImpactMeter.xaml
    /// </summary>
    public partial class AnalogImpactMeter : UserControl
    {
        private const int NEEDLE_RESET_DELAY_MIN = 3000;
        private const int NEEDLE_RESET_DELAY_MAX = 5000;
        private DispatcherTimer _needleResetTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        private Random _rnd = new Random();
        private double _defaultMax;

        public AnalogImpactMeter()
        {
            InitializeComponent();
            _defaultMax = this.Scale.Max;
            _needleResetTimer.Tick += NeedleResetTimer_Tick;
        }

        #region Event Handlers

        /// <summary>
        /// Handles the Tick event of the NeedleResetTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void NeedleResetTimer_Tick(object sender, EventArgs e)
        {
            _needleResetTimer.Stop();
            Needle.Value = 0;
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the impact level.
        /// </summary>
        /// <value>The impact level.</value>
        public byte ImpactLevel
        {
            get { return (byte)GetValue(ImpactLevelProperty); }
            set { SetValue(ImpactLevelProperty, value); }
        }

        public static readonly DependencyProperty ImpactLevelProperty =
           DependencyProperty.Register(
              "ImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeImpactLevel)));

        /// <summary>
        /// Changes the impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as AnalogImpactMeter).UpdateImpactLevel((byte)e.NewValue);
        }

        /// <summary>
        /// Updates the impact level.
        /// </summary>
        /// <param name="NewImpactLevel">The new impact level.</param>
        private void UpdateImpactLevel(byte NewImpactLevel)
        {
            if (NewImpactLevel > this.Scale.Max) this.Scale.Max = NewImpactLevel;
            this.Needle.Value = ImpactLevel;

            _needleResetTimer.Interval = TimeSpan.FromMilliseconds(_rnd.Next(NEEDLE_RESET_DELAY_MIN, NEEDLE_RESET_DELAY_MAX));
            _needleResetTimer.Start();
        }

        /// <summary>
        /// Gets or sets the Required impact level.
        /// </summary>
        /// <value>The impact level.</value>
        public byte RequiredImpactLevel
        {
            get { return (byte)GetValue(RequiredImpactLevelProperty); }
            set { SetValue(RequiredImpactLevelProperty, value); }
        }

        public static readonly DependencyProperty RequiredImpactLevelProperty =
           DependencyProperty.Register(
              "RequiredImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeRequiredImpactLevel)));

        /// <summary>
        /// Changes the Required impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeRequiredImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as AnalogImpactMeter).UpdateRequiredImpactLevel((byte)e.NewValue);
        }

        /// <summary>
        /// Updates the Required impact level.
        /// </summary>
        /// <param name="NewImpactLevel">The new impact level.</param>
        private void UpdateRequiredImpactLevel(byte NewImpactLevel)
        {
            this.Scale.Max = Math.Max(NewImpactLevel, Math.Max(HighestImpactLevel, _defaultMax));
            this.RadialBar.Value = NewImpactLevel;
        }

        /// <summary>
        /// Gets or sets the Highest impact level.
        /// </summary>
        /// <value>The impact level.</value>
        public byte HighestImpactLevel
        {
            get { return (byte)GetValue(HighestImpactLevelProperty); }
            set { SetValue(HighestImpactLevelProperty, value); }
        }

        public static readonly DependencyProperty HighestImpactLevelProperty =
           DependencyProperty.Register(
              "HighestImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeHighestImpactLevel)));

        /// <summary>
        /// Changes the Highest impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeHighestImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                (source as AnalogImpactMeter).UpdateHighestImpactLevel((byte)e.NewValue);
        }

        /// <summary>
        /// Updates the Highest impact level.
        /// </summary>
        /// <param name="NewImpactLevel">The new impact level.</param>
        private void UpdateHighestImpactLevel(byte NewImpactLevel)
        {
            this.Scale.Max = Math.Max(NewImpactLevel, Math.Max(RequiredImpactLevel, _defaultMax));
            this.Marker.Value = NewImpactLevel;
        }

        #endregion
    }
}
