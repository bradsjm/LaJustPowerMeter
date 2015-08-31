namespace LaJust.PowerMeter.Common.BaseClasses
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows;

    /// <summary>
    /// Base presenter class for a presenter
    /// </summary>
    public class Presenter : PropertyNotifier
    {
        /// <summary>
        /// Called to request closing/removal of this presenter by the owner
        /// </summary>
        public event EventHandler RequestRemoval = delegate { };

        #region Event Raising Methods

        /// <summary>
        /// Called when presenter requests to be removed by owner.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected virtual void OnRequestRemoval()
        {
            EventHandler handler = RequestRemoval;
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Trace.TraceError("OnRequestRemoval: {0}", ex.GetBaseException());
            }
        }

        #endregion
    }
}
