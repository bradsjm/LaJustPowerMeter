// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExportAttribute.cs" company="LaJust Sports America">
//   LaJust Sports America, All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Infrastructure
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// View Export Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public class ViewExportAttribute : ExportAttribute, IViewRegionRegistration
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewExportAttribute"/> class.
        /// </summary>
        public ViewExportAttribute()
            : base(typeof(object))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewExportAttribute"/> class.
        /// </summary>
        /// <param name="viewName">
        /// Name of the view.
        /// </param>
        public ViewExportAttribute(string viewName)
            : base(viewName, typeof(object))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the region.
        /// </summary>
        /// <value>The name of the region.</value>
        public string RegionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        /// <value>The name of the view.</value>
        public string ViewName { get; set; }

        #endregion
    }
}