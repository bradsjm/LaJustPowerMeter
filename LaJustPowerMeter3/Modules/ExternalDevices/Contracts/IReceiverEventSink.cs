namespace ExternalDevices
{
    using System;
    using LaJust.EIDSS.Communications;
    using System.ComponentModel;

    public interface IReceiverEventSink
    {
        /// <summary>
        /// Wires up receiver manager.
        /// </summary>
        /// <param name="receiverManager">The receiver manager.</param>
        void Run(IReceiverManager receiverManager);
    }
}
