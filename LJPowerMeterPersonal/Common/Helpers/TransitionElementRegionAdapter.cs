namespace LaJust.PowerMeter.Common.Helpers
{
    using System;
    using System.Linq;
    using System.Windows.Data;
    using Microsoft.Practices.Composite.Presentation.Regions;
    using Microsoft.Practices.Composite.Regions;
    using Transitionals.Controls;
    using System.Collections.Specialized;

    /// <summary>
    /// Adapts the TranstionElement as a Region
    /// </summary>
    public class TransitionElementRegionAdapter : RegionAdapterBase<TransitionElement>
    {
        private IRegion _region;
        private TransitionElement _regionTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionElementRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TransitionElementRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory) { }

        /// <summary>
        /// Template method to create a new instance of <see cref="T:Microsoft.Practices.Composite.Regions.IRegion"/>
        /// that will be used to adapt the object.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:Microsoft.Practices.Composite.Regions.IRegion"/>.
        /// </returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

        /// <summary>
        /// Template method to adapt the object to an <see cref="T:Microsoft.Practices.Composite.Regions.IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, TransitionElement regionTarget)
        {
            if (regionTarget.Content != null ||
                (BindingOperations.GetBinding(regionTarget, TransitionElement.ContentProperty) != null))
                throw new InvalidOperationException();
            _region = region;
            _regionTarget = regionTarget;
            region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the ActiveViews control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _regionTarget.TransitionsEnabled = (_regionTarget.Content != null);
                _regionTarget.Content = _region.ActiveViews.FirstOrDefault();
            }
        }
    }
}
