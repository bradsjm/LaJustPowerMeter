// <copyright file="SimpleGraph.cs" company="LaJust Sports America">
// Copyright (c) 2010 All Rights Reserved.
// </copyright>
// <author>Jonathan Bradshaw</author>
// <email>jonathan.bradshaw@lajustsports.com</email>

namespace ImpactMeter
{
    using System;
    using Infrastructure;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// A simple WPF Ticker graph, that displays all GraphDataItems
    /// within the ObservableCollection<GraphDataItem> property of this
    /// control.
    /// </summary>
    public partial class SimpleGraph : UserControl
    {
        #region Private Fields

        const int MAX_POINTS = 15;

        const double DEFAULT_MAX = 100;

        private PointCollection _graphPoints = new PointCollection();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleGraph"/> class.
        /// </summary>
        public SimpleGraph()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateThresholdLine();
            UpdateLineGraph();
        }

        /// <summary>
        /// Updates the threshold line.
        /// </summary>
        private void UpdateThresholdLine()
        {
            if (this.Threshold > 0 && container.ActualHeight > 0 && this.MaxValue > this.MinValue)
            {
                double scale = this.MaxValue - this.MinValue;
                double valuePerPoint = container.ActualHeight / scale;
                this.ThresholdLine.Y1 = container.ActualHeight - ((Threshold - MinValue) * valuePerPoint);
                this.ThresholdLine.Y2 = ThresholdLine.Y1;
                this.ThresholdLine.Visibility = Visibility.Visible;
            }
            else
            {
                this.ThresholdLine.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Works out the actual X/Y points for the each value within the
        /// data and draws the line graph.
        /// </summary>
        private void UpdateLineGraph()
        {
            // Only proceed if there are some actual values and there are enough values
            if (this.DataValues == null || this.DataValues.Count < 2 || container.ActualHeight == 0 || this.MaxValue <= this.MinValue)
            {
                GraphLine.Visibility = Visibility.Hidden;
                LastPointMarkerEllipse.Visibility = Visibility.Hidden;
                ScaleCurrentValue.Content = string.Empty;
                return;
            }

            int count = this.DataValues.Count;
            double max = Math.Max(this.Threshold, this.DataValues.Max());
            this.MaxValue = Math.Max(DEFAULT_MAX, max);

            double scale = this.MaxValue - this.MinValue;
            double valuePerPoint = container.ActualHeight / scale;
            double constantOffset = container.ActualWidth / Math.Min(MAX_POINTS, count);
            double xOffSet = 0;

            // For each item work out what the actual X/Y should be 
            _graphPoints.Clear();
            int start = count > MAX_POINTS ? count - MAX_POINTS : 0;
            for (int i = start; i < count; i++)
            {
                double trueDiff = this.DataValues[i] - this.MinValue;
                double heightPx = trueDiff * valuePerPoint;
                double yValue = container.ActualHeight - heightPx;

                _graphPoints.Add(new Point(xOffSet, yValue));
                xOffSet += constantOffset;
            }

            // Add Polygon Points
            GraphLine.Points = _graphPoints;

            // set LastPointMarkerEllipse
            Point lastPoint = _graphPoints.Last();
            LastPointMarkerEllipse.SetValue(Canvas.LeftProperty, lastPoint.X - (LastPointMarkerEllipse.Width / 2.0));
            LastPointMarkerEllipse.SetValue(Canvas.TopProperty, lastPoint.Y - (LastPointMarkerEllipse.Height / 2.0));
            LastPointMarkerEllipse.Visibility = Visibility.Visible;

            // Set label
            ScaleCurrentValue.Content = this.DataValues.Last().ToString("N0");
            ScaleCurrentValue.SetValue(Canvas.LeftProperty, lastPoint.X - (LastPointMarkerEllipse.Width * 2.0));
            if (lastPoint.Y < (GraphLine.ActualHeight / 2.0))
            {
                ScaleCurrentValue.SetValue(Canvas.TopProperty, lastPoint.Y + LastPointMarkerEllipse.Height);
            }
            else
            {
                ScaleCurrentValue.SetValue(Canvas.TopProperty, lastPoint.Y - (LastPointMarkerEllipse.Height * 3.0));
            }

            // Got points now so show graph
            GraphLine.Visibility = Visibility.Visible;
        }

        #endregion

        #region DataValues

        /// <summary>
        /// Dependency Property for Data Values
        /// </summary>
        public static readonly DependencyProperty DataValuesProperty =
            DependencyProperty.Register(
                "DataValues",
                typeof(DispatchingObservableCollection<byte>), 
                typeof(SimpleGraph),
                new UIPropertyMetadata((DispatchingObservableCollection<byte>)null,
                    new PropertyChangedCallback(OnDataValuesChanged)));

        /// <summary>
        /// Gets or sets the data values.
        /// </summary>
        /// <value>The data values.</value>
        public DispatchingObservableCollection<byte> DataValues
        {
            get { return (DispatchingObservableCollection<byte>)GetValue(DataValuesProperty); }
            set { SetValue(DataValuesProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DataValues property.
        /// </summary>
        private static void OnDataValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleGraph instance = (SimpleGraph)d;
            instance.DataValues = (DispatchingObservableCollection<byte>)e.NewValue;
            instance.DataValues.CollectionChanged += instance.DataValues_CollectionChanged;
            instance.UpdateLineGraph();
        }

        /// <summary>
        /// Handles the CollectionChanged event of the DataValues control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateLineGraph();
        }
        #endregion

        #region MinValue

        /// <summary>
        /// MinValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue", 
                typeof(double),
                typeof(SimpleGraph),
                new UIPropertyMetadata((double)0,
                    new PropertyChangedCallback(OnMinValueChanged)));

        /// <summary>
        /// Gets or sets the MinValue property.  
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// Called when [min value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleGraph graph = (SimpleGraph)d;

            if (e.NewValue != null)
            {
                graph.y1Label.Content = graph.MaxValue.ToString("N0");
                graph.y2Label.Content = (graph.MaxValue / 2).ToString("N0");
                graph.y3Label.Content = graph.MinValue.ToString("N0");
                graph.UpdateThresholdLine();
            }
        }

        #endregion

        #region MaxValue

        /// <summary>
        /// MaxValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                "MaxValue", 
                typeof(double), 
                typeof(SimpleGraph),
                new UIPropertyMetadata((double)DEFAULT_MAX,
                    new PropertyChangedCallback(OnMaxValueChanged)));

        /// <summary>
        /// Gets or sets the MaxValue property.  
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// Called when [max value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleGraph graph = (SimpleGraph)d;

            if (e.NewValue != null)
            {
                graph.y1Label.Content = graph.MaxValue.ToString("N0");
                graph.y2Label.Content = (graph.MaxValue / 2).ToString("N0");
                graph.y3Label.Content = graph.MinValue.ToString("N0");
                graph.UpdateThresholdLine();
            }
        }

        #endregion

        #region Threshold

        /// <summary>
        /// Threshold Dependency Property
        /// </summary>
        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(
                "Threshold",
                typeof(double),
                typeof(SimpleGraph),
                new UIPropertyMetadata((double)0,
                    new PropertyChangedCallback(OnThresholdValueChanged)));

        /// <summary>
        /// Gets or sets the Threshold property.  
        /// </summary>
        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        /// <summary>
        /// Called when [threshold value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnThresholdValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleGraph graph = (SimpleGraph)d;

            if (e.NewValue != null)
            {
                graph.UpdateThresholdLine();
            }
        }

        #endregion

        #region LineColor

        /// <summary>
        /// Threshold Dependency Property
        /// </summary>
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register(
                "LineColor",
                typeof(Brush), 
                typeof(SimpleGraph),
                new UIPropertyMetadata((Brush)Brushes.White,
                    new PropertyChangedCallback(OnLineColorValueChanged)));

        /// <summary>
        /// Gets or sets the Threshold property.  
        /// </summary>
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        /// <summary>
        /// Called when [line color value changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLineColorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleGraph graph = (SimpleGraph)d;

            if (e.NewValue != null)
            {
                graph.GraphLine.Stroke = (Brush)e.NewValue;
            }
        }

        #endregion
    }
}
