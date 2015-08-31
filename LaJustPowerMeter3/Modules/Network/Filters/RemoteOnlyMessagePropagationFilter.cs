namespace Network
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class RemoteOnlyMessagePropagationFilter : PeerMessagePropagationFilter
    {
        /// <summary>
        /// Returns whether or not a message received on a peer channel should be propagated, and if so, the destination of the message.
        /// </summary>
        /// <param name="message">The message to evaluate for propagation.</param>
        /// <param name="origination">A <see cref="T:System.ServiceModel.PeerMessageOrigination"/> enumeration value that specifies the origin (local or remote) of the message under evaluation.</param>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.PeerMessagePropagation"/> enumeration value that indicates the destination of the message (local, remote, both, or no propagation at all).
        /// </returns>
        public override PeerMessagePropagation ShouldMessagePropagate(Message message, PeerMessageOrigination origination)
        {
            PeerMessagePropagation destination = PeerMessagePropagation.LocalAndRemote;
            if (origination == PeerMessageOrigination.Local)
                destination = PeerMessagePropagation.Remote;
            return destination;
        }
    }
}