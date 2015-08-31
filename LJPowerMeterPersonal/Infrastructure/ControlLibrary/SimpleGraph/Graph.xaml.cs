using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Specialized;

namespace LaJust.PowerMeter.ControlLibrary.SimpleGraph
{
    /// <summary>
    /// A simple WPF Ticker graph, that displays all GraphDataItems
    /// within the ObservableCollection<GraphDataItem> property of this
    /// control. The X-Axis is obtained by taking the time period between
    /// the 1st and last GraphDataItem within the 
    /// ObservableCollection<GraphDataItem> property.
    /// 
    /// Whilst the Y scale uses the Min/Max of all items within the
    /// ObservableCollection<GraphDataItem> property
    /// 
    /// The new data will only be added to to the graph via an ObservableCollection
    /// if the Graph.CanAcceptNewStreamingReadings is set to true.
    /// </summary>
    public partial class Graph : UserControl
    {
        #region Private Fields

        const int MIN_ITEMS_TO_PLOT = 2;

        private PointCollection _graphPoints = new PointCollection();

        #endregion

        #region Constructor

        public Graph()
        {
            InitializeComponent();
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
            ObtainPointsForValues();
        }

        /// <summary>
        /// Updates the threshold line.
        /// </summary>
        private void UpdateThresholdLine()
        {
            if (Threshold > 0 && container.ActualHeight > 0)
            {
                double scale = MaxValue - MinValue;
                double valuePerPoint = container.ActualHeight / scale;
                ThresholdLine.Y1 = container.ActualHeight - ((Threshold - MinValue) * valuePerPoint);
                ThresholdLine.Y2 = ThresholdLine.Y1;
                ThresholdLine.Visibility = Visibility.Visible;
            }
            else
            {
                ThresholdLine.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Works out the actual X/Y points for the each value within the
        /// ObservableCollection<GraphDataItem> property.
        /// </summary>
        private void ObtainPointsForValues()
        {
            //Only proceed if there are some actual values and there are enough values
            if (DataValues == null || container.ActualHeight == 0 || DataValues.Count < MIN_ITEMS_TO_PLOT)
            {
                GraphLine.Visibility = Visibility.Hidden;
                LastPointMarkerEllipse.Visibility = Visibility.Hidden;
                ScaleCurrentValue.Content = string.Empty;
                return;
            }
            else
            {
                #region Workout Points
                double scale = MaxValue - MinValue;
                double valuePerPoint = container.ActualHeight / scale;
                double constantOffset = container.ActualWidth / DataValues.Count;
                double xOffSet = 0;

                ///for each item seen work out what the actual X/Y should be 
                ///based on a bit of Maths
                _graphPoints.Clear();
                for (int i = 0; i < DataValues.Count; i++)
                {
                    double trueDiff = DataValues[i] - MinValue;
                    double heightPx = trueDiff * valuePerPoint;
                    double yValue = container.ActualHeight - heightPx;

                    _graphPoints.Add(new Point(xOffSet, yValue));
                    xOffSet += constantOffset;
                }

                Point lastPoint = _graphPoints.Last();

                //set LastPointMarkerEllipse
                LastPointMarkerEllipse.SetValue(Canvas.LeftProperty, lastPoint.X - (LastPointMarkerEllipse.Width / 2));
                LastPointMarkerEllipse.SetValue(Canvas.TopProperty, lastPoint.Y - (LastPointMarkerEllipse.Height / 2));
                LastPointMarkerEllipse.Visibility = Visibility.Visible;

                #endregion

                #region Label Current Data Point
                ScaleCurrentValue.Content = this.DataValues.Last().ToString("N0");
                ScaleCurrentValue.SetValue(Canvas.LeftProperty, lastPoint.X - (LastPointMarkerEllipse.Width * 2));
                ScaleCurrentValue.SetValue(Canvas.TopProperty, lastPoint.Y - (LastPointMarkerEllipse.Height * 2));
                #endregion

                //Add Polygon Points
                GraphLine.Points = _graphPoints;

                //Got points now so show graph
                GraphLine.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// When the current collection of GraphDataItem changes, work out
        /// the actual X/Y point values based on the actual values
        /// </summary>
        private void DataValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObtainPointsForValues();
        }

        #endregion

        #region Dependency Properties

        #region DataValues

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataValuesProperty =
            DependencyProperty.Register("DataValues",
            typeof(GraphDataCollection), typeof(Graph),
                new UIPropertyMetadata((GraphDataCollection)null,
                    new PropertyChangedCallback(OnDataValuesChanged)));

        /// <summary>
        /// Gets or sets the data values.
        /// </summary>
        /// <value>The data values.</value>
        public GraphDataCollection DataValues
        {
            get { return (GraphDataCollection)GetValue(DataValuesProperty); }
            set { SetValue(DataValuesProperty, value); }
        }

        /// <summary>
        /// Handles changes to the DataValues property.
        /// </summary>
        private static void OnDataValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Graph obj = (Graph)d;

            if (e.NewValue != null && e.OldValue != null)
            {
                //if we see a different actual ThreadSafeObservableCollection<GraphDataItem>
                //we should unhook the old event handler
                if (!Object.ReferenceEquals(e.OldValue, e.NewValue))
                {
                    GraphDataCollection oldDataValues = (GraphDataCollection)e.OldValue;
                    oldDataValues.CollectionChanged -= obj.DataValues_CollectionChanged;

                    GraphDataCollection newDataValues = (GraphDataCollection)e.OldValue;
                    newDataValues.CollectionChanged += obj.DataValues_CollectionChanged;
                }
                else
                {
                    GraphDataCollection newDataValues = (GraphDataCollection)e.NewValue;
                    newDataValues.CollectionChanged += obj.DataValues_CollectionChanged;
                }
            }
            else
            {
                GraphDataCollection newDataValues = (GraphDataCollection)e.NewValue;
                newDataValues.CollectionChanged += obj.DataValues_CollectionChanged;
            }
        }
        #endregion

        #region MinValue

        /// <summary>
        /// MinValue Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(Graph),
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
            Graph graph = (Graph)d;

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
            DependencyProperty.Register("MaxValue", typeof(double), typeof(Graph),
                new UIPropertyMetadata((double)double.MaxValue,
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
            Graph graph = (Graph)d;

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
            DependencyProperty.Register("Threshold", typeof(double), typeof(Graph),
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
            Graph graph = (Graph)d;

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
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(Graph),
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
            Graph graph = (Graph)d;

            if (e.NewValue != null)
            {
                graph.GraphLine.Stroke = (Brush)e.NewValue;
            }
        }

        #endregion

        #endregion
    }
}
