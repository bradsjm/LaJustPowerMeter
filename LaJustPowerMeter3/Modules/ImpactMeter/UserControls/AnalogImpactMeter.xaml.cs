// <copyright file="AnalogImpactMeter.xaml.cs" company="LaJust Sports America">
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
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for AnalogImpactMeter.xaml
    /// </summary>
    public partial class AnalogImpactMeter : UserControl
    {
        /// <summary>
        /// Required Impact Level Dependency Property
        /// </summary>
        public static readonly DependencyProperty RequiredImpactLevelProperty =
           DependencyProperty.Register(
              "RequiredImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeRequiredImpactLevel)));

        /// <summary>
        /// Highest Impact Level Dependency Property
        /// </summary>
        public static readonly DependencyProperty HighestImpactLevelProperty =
           DependencyProperty.Register(
              "HighestImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeHighestImpactLevel)));

        /// <summary>
        /// Impact Level Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImpactLevelProperty =
           DependencyProperty.Register(
              "ImpactLevel",
              typeof(byte),
              typeof(AnalogImpactMeter),
              new FrameworkPropertyMetadata((byte)0, new PropertyChangedCallback(ChangeImpactLevel)));

        /// <summary>
        /// Minimum delay time before needle will reset to zero
        /// </summary>
        private const int NeedleResetDelayMinimum = 7500;

        /// <summary>
        /// Maximum delay time before the needle will reset to zero
        /// </summary>
        private const int NeedleResetDelayMaximum = 15000;

        /// <summary>
        /// Needle Reset Dispatching Timer, we only reset when the application is idle
        /// </summary>
        private readonly DispatcherTimer NeedleResetTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

        /// <summary>
        /// Random Number Generator
        /// </summary>
        private readonly Random Rnd = new Random();

        /// <summary>
        /// The default maximum value set in the XAML
        /// </summary>
        private double defaultMaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalogImpactMeter"/> class.
        /// </summary>
        public AnalogImpactMeter()
        {
            InitializeComponent();
            this.defaultMaxValue = this.Scale.Max;
            this.NeedleResetTimer.Tick += this.NeedleResetTimer_Tick;
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

        /// <summary>
        /// Gets or sets the Highest impact level.
        /// </summary>
        /// <value>The impact level.</value>
        public byte HighestImpactLevel
        {
            get { return (byte)GetValue(HighestImpactLevelProperty); }
            set { SetValue(HighestImpactLevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the impact level.
        /// </summary>
        /// <value>The impact level.</value>
        public byte ImpactLevel
        {
            get { return (byte)GetValue(ImpactLevelProperty); }
            set { SetValue(ImpactLevelProperty, value); }
        }

        #region Dependency Property Implementation

        /// <summary>
        /// Changes the impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as AnalogImpactMeter).UpdateImpactLevel((byte)e.NewValue);
            }
        }

        /// <summary>
        /// Changes the Required impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeRequiredImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as AnalogImpactMeter).UpdateRequiredImpactLevel((byte)e.NewValue);
            }
        }

        /// <summary>
        /// Changes the Highest impact level.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ChangeHighestImpactLevel(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (source as AnalogImpactMeter).UpdateHighestImpactLevel((byte)e.NewValue);
            }
        }

        /// <summary>
        /// Updates the impact level.
        /// </summary>
        /// <param name="newImpactLevel">The new impact level.</param>
        private void UpdateImpactLevel(byte newImpactLevel)
        {
            if (newImpactLevel > this.Scale.Max)
            {
                this.Scale.Max = newImpactLevel;
            }

            this.Needle.Value = newImpactLevel;

            // Reset the timer each time we get a change in the impact level
            this.NeedleResetTimer.Interval = TimeSpan.FromMilliseconds(
                this.Rnd.Next(NeedleResetDelayMinimum, NeedleResetDelayMaximum));
            this.NeedleResetTimer.Start();
        }

        /// <summary>
        /// Updates the Required impact level.
        /// </summary>
        /// <param name="newImpactLevel">The new impact level.</param>
        private void UpdateRequiredImpactLevel(byte newImpactLevel)
        {
            this.Scale.Max = Math.Max(newImpactLevel, Math.Max(this.HighestImpactLevel, this.defaultMaxValue));
            this.RadialBar.Value = newImpactLevel;
        }

        /// <summary>
        /// Updates the Highest impact level.
        /// </summary>
        /// <param name="newImpactLevel">The new impact level.</param>
        private void UpdateHighestImpactLevel(byte newImpactLevel)
        {
            this.Scale.Max = Math.Max(newImpactLevel, Math.Max(this.RequiredImpactLevel, this.defaultMaxValue));
            this.Marker.Value = newImpactLevel;
        }

        #endregion

        /// <summary>
        /// Handles the Tick event of the NeedleResetTimer to reset the needle back to zero.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void NeedleResetTimer_Tick(object sender, EventArgs e)
        {
            this.NeedleResetTimer.Stop();
            this.Needle.Value = 0;
        }
    }
}
