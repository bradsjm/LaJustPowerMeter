namespace ExternalDevices
{
    using System;
    using LaJust.EIDSS.Communications;
    using System.ComponentModel;

    public interface IReceiverEventPublisher : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets the receiver count.
        /// </summary>
        /// <value>The receiver count.</value>
        int ReceiverCount { get; }

        /// <summary>
        /// Wires up receiver manager.
        /// </summary>
        /// <param name="receiverManager">The receiver manager.</param>
        void Run(IReceiverManager receiverManager);
    }
}
