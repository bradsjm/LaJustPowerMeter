namespace InterAppComms
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using InterAppComms.Contracts;
    using InterAppComms.Services;
    using System.Diagnostics;

    /// <summary>
    /// Mesh Sender Communication Class
    /// </summary>
    public class MeshSender<T> : SenderBase<T> where T : IBasicService
    {
        /// <summary>
        /// Mesh URI
        /// </summary>
        const string MeshBaseUri = "net.p2p://";

        /// <summary>
        /// Private mesh password
        /// </summary>
        const string MeshPassword = "cSwPC9EQBUyizs7c993HEvTZ109e57e0";

        private class RemoteOnlyMessagePropagationFilter : PeerMessagePropagationFilter
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

        protected override ChannelFactory<T> CreateChannel(string assemblyGuid, string name)
        {
            var endPointAddress = new EndpointAddress(MeshBaseUri + assemblyGuid + "/" + name);
            var channelFactory = new ChannelFactory<T>(new NetPeerTcpBinding(), endPointAddress);
            channelFactory.Credentials.Peer.MeshPassword = MeshPassword;
            var peerNode = ((IClientChannel)this.Proxy).GetProperty<PeerNode>();
            peerNode.MessagePropagationFilter = new RemoteOnlyMessagePropagationFilter();
            Trace.WriteLine("Creating WCF Service Sender Host at " + endPointAddress);
            channelFactory.Open();
            return channelFactory;
        }
    }
}
