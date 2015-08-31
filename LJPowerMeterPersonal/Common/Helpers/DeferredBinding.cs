namespace LaJust.PowerMeter.Common.Helpers
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
    ///       <DeferredBinder:DeferredBinding x:Key="dbTest" Delay="00:00:01" MaxDelay="00:00:05" />
    ///    </StackPanel.Resources>
    ///    <TextBlock x:Name="tb" Text="{Binding Source={StaticResource dbTest}, Path=Target}" />
    ///    <Slider x:Name="slider" Value="{Binding Source={StaticResource dbTest}, Path=Source, Mode=OneWayToSource}" />
    /// </StackPanel>
    /// ]]>
    /// </example>
    public class DeferredBinding : Freezable
    {
        #region Private Members
        /// <summary>
        /// Timer used to track value changes
        /// </summary>
        private DispatcherTimer _timer;
        /// <summary>
        /// DateTime used to track the last update
        /// </summary>
        private DateTime _lastUpdate = DateTime.MinValue;
        #endregion

        #region MaxDelay Dependency Property

        /// <summary>
        /// Timeout Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty MaxDelayProperty =
            DependencyProperty.Register("MaxDelay", typeof(TimeSpan), typeof(DeferredBinding),
                new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the MaxDelay property.
        /// </summary>
        public TimeSpan MaxDelay
        {
            get { return (TimeSpan)GetValue(MaxDelayProperty); }
            set { SetValue(MaxDelayProperty, value); }
        }

        #endregion

        #region Delay Dependency Property

        /// <summary>
        /// Timeout Dependency Property - controls how long we wait until value is transferred.
        /// </summary>
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(TimeSpan), typeof(DeferredBinding),
                new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.None, OnDelayChanged));

        /// <summary>
        /// Gets or sets the MinDelay property.
        /// </summary>
        public TimeSpan Delay
        {
            get { return (TimeSpan)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        /// <summary>
        /// Handles changes to the MinDelay property.
        /// </summary>
        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinding)d).ResetTimer();
        }

        #endregion

        #region Source Binding Dependency Property

        /// <summary>
        /// Source Dependency Property - this is SOURCE of the property change
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(DeferredBinding),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, OnSourceChanged));

        /// <summary>
        /// Gets or sets the Source property.  
        /// </summary>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Target property.
        /// </summary>
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeferredBinding)d).CheckLastUpdate();
        }

        #endregion

        #region Target Binding Dependency Property

        /// <summary>
        /// Target Dependency Property
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof (object), typeof (DeferredBinding),
               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the Target property.  This dependency property 
        /// indicates ....
        /// </summary>
        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Checks the last update.
        /// </summary>
        private void CheckLastUpdate()
        {
            if (MaxDelay != TimeSpan.Zero && DateTime.Now > _lastUpdate + MaxDelay)
            {
                OnTimeout(this, EventArgs.Empty);
            }
            else
            {
                ResetTimer();
            }
        }

        /// <summary>
        /// This resets the timer.
        /// </summary>
        private void ResetTimer()
        {
            if (Delay == TimeSpan.Zero)
            {
                _timer = null;
                return;
            }

            if (_timer != null)
            {
                _timer.IsEnabled = false;
                _timer.Interval = Delay;
                _timer.IsEnabled = true;
            }
            else
            {
                _timer = new DispatcherTimer(Delay, 
                    DispatcherPriority.ContextIdle, OnTimeout, this.Dispatcher) { IsEnabled = true };
            }
        }

        /// <summary>
        /// This is called when the timeout occurs and it transfers the source to the target.
        /// </summary>
        /// <param name="sender">Dispatcher Timer</param>
        /// <param name="e">Event arguments</param>
        private void OnTimeout(object sender, EventArgs e)
        {
            _timer.IsEnabled = false;
            _lastUpdate = DateTime.Now;
            this.Target = this.Source;
        }

        #endregion

        #region Overridden Freezable Class Methods
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
        #endregion
    }
}
