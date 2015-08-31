namespace LaJust.PowerMeter.Common.BaseClasses
{
    /// <summary>
    /// Generic IView interface that all views must implement
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Applies the presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        void ApplyPresenter(object presenter);
    }
}
