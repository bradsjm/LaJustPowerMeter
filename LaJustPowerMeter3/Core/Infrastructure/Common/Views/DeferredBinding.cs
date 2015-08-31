// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferredBinding.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// This class provides a simple deferred wrapper which binds two properties together and 
    /// delays the transfer until Delay (using a dispatcher timer). You can also (or instead) 
    /// specify MaxDelay which does an immediate transfer if the previous one was over the MaxDelay.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <StackPanel>
    ///    <StackPanel.Resources>
    ///       <DeferredBinder:DeferredBinding x:Key="dbTest" Delay="00:00:01" MaxDelay="00:00:05"/>
    ///    </StackPanel.Resources>
    ///    <TextBlock x:Name="tb" Text="{Binding Source={StaticResource dbTest}, Path=Target}"/>
    ///    <Slider x:Name="slider" Value="{Binding Source={StaticResource dbTest}, Path=Source, Mode=OneWayToSource}"/>
    /// </StackPanel>
    /// ]]>
    /// </example>
    public class DeferredBinding : Freezable
    {
        #region Constants and Fields

        /// <summary>
        /// Delay Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            "Delay", 
            typeof(TimeSpan), 
            typeof(DeferredBinding), 
            new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.None, OnDelayChanged));

        /// <summary>
        /// MaxDelay Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty MaxDelayProperty = DependencyProperty.Register(
            "MaxDelay", 
            typeof(TimeSpan), 
            typeof(DeferredBinding), 
            new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Source Dependency Property - this is SOURCE of the property change
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", 
            typeof(object), 
            typeof(DeferredBinding), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSourceChanged));

        /// <summary>
        /// Target Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", 
            typeof(object), 
            typeof(DeferredBinding), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// DateTime used to track the last update
        /// </summary>
        private DateTime lastUpdate = DateTime.MinValue;

        /// <summary>
        /// Timer used to track value changes
        /// </summary>
        private DispatcherTimer timer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Delay property.
        /// </summary>
        /// <value>The delay.</value>
        public TimeSpan Delay
        {
            get
            {
                return (TimeSpan)this.GetValue(DelayProperty);
            }

            set
            {
                this.SetValue(DelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the MaxDelay property.
        /// </summary>
        public TimeSpan MaxDelay
        {
            get
            {
                return (TimeSpan)this.GetValue(MaxDelayProperty);
            }

            set
            {
                this.SetValue(MaxDelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Source property.  
        /// </summary>
        public object Source
        {
            get
            {
                return this.GetValue(SourceProperty);
            }

            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Target property.
        /// </summary>
        public object Target
        {
            get
            {
                return this.GetValue(TargetProperty);
            }

            set
            {
                this.SetValue(TargetProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles changes to the Delay property.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinding)d).ResetTimer();
        }

        /// <summary>
        /// Handles changes to the Target property.
        /// </summary>
        /// <param name="d">
        /// The dependency object.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinding)d).CheckLastUpdate();
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class. 
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This is called when the timeout occurs and it transfers the source to the target.
        /// </summary>
        /// <param name="sender">
        /// Dispatcher Timer
        /// </param>
        /// <param name="e">
        /// Event arguments
        /// </param>
        protected void OnTimeout(object sender, EventArgs e)
        {
            this.timer.IsEnabled = false;
            this.lastUpdate = DateTime.Now;
            this.Target = this.Source;
        }

        /// <summary>
        /// Checks the last update.
        /// </summary>
        private void CheckLastUpdate()
        {
            if (this.MaxDelay != TimeSpan.Zero && DateTime.Now > this.lastUpdate + this.MaxDelay)
            {
                this.OnTimeout(this, EventArgs.Empty);
            }
            else
            {
                this.ResetTimer();
            }
        }

        /// <summary>
        /// This resets the timer.
        /// </summary>
        private void ResetTimer()
        {
            if (this.Delay == TimeSpan.Zero)
            {
                this.timer = null;
                return;
            }

            if (this.timer != null)
            {
                this.timer.IsEnabled = false;
                this.timer.Interval = this.Delay;
                this.timer.IsEnabled = true;
            }
            else
            {
                this.timer = new DispatcherTimer(
                    this.Delay, DispatcherPriority.ContextIdle, this.OnTimeout, this.Dispatcher)
                    {
                        IsEnabled = true 
                    };
            }
        }

        #endregion
    }
}