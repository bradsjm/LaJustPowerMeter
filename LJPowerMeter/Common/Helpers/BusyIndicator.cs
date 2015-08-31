namespace LaJust.PowerMeter.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;

    /// <summary>
    /// USAGE:
    /// using (new ShowBusyIndicator())
    /// {
    ///     Do work...
    /// }
    /// </summary>
    public class ShowBusyIndicator : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowBusyIndicator"/> class.
        /// </summary>
        public ShowBusyIndicator()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Mouse.OverrideCursor = null;
        }

        #endregion
    }
}
