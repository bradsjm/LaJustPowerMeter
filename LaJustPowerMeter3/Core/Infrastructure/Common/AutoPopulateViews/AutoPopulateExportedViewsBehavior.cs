// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoPopulateExportedViewsBehavior.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel.Composition;

    using Microsoft.Practices.Prism.Regions;

    /// <summary>
    /// Auto Populates Exported Views to Regions
    /// </summary>
    [Export(typeof(AutoPopulateExportedViewsBehavior))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AutoPopulateExportedViewsBehavior : RegionBehavior, IPartImportsSatisfiedNotification
    {
        #region Properties

        /// <summary>
        /// Gets or sets the registered views.
        /// </summary>
        /// <value>The registered views.</value>
        [ImportMany(AllowRecomposition = true)]
        public Lazy<object, IViewRegionRegistration>[] RegisteredViews { get; set; }

        #endregion

        #region IPartImportsSatisfiedNotification

        /// <summary>
        /// Called when a part's imports have been satisfied and it is safe to use.
        /// </summary>
        public void OnImportsSatisfied()
        {
            this.AddRegisteredViews();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override this method to perform the logic after the behavior has been attached.
        /// </summary>
        protected override void OnAttach()
        {
            this.AddRegisteredViews();
        }

        /// <summary>
        /// Adds the registered views.
        /// </summary>
        private void AddRegisteredViews()
        {
            if (this.Region != null)
            {
                foreach (var viewEntry in this.RegisteredViews)
                {
                    if (viewEntry.Metadata.RegionName == this.Region.Name)
                    {
                        object view = viewEntry.Value;

                        if (!this.Region.Views.Contains(view))
                        {
                            if (string.IsNullOrEmpty(viewEntry.Metadata.ViewName))
                            {
                                this.Region.Add(view);
                            }
                            else
                            {
                                this.Region.Add(view, viewEntry.Metadata.ViewName);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}