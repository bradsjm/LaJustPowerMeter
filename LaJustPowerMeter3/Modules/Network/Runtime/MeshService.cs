namespace Network
{
    using System;
    using System.ServiceModel;
    using System.Threading;

    /// <summary>
    /// WCF Server Wrapper
    /// </summary>
    public class MeshService : IDisposable
    {
        #region Private Constants

        /// <summary>
        /// The private mesh password
        /// </summary>
        private const string MeshPassword = "cSwPC9EQBUyizs7c993HEvTZ109e57e0";

        /// <summary>
        /// Time out after which we assume a node is gone
        /// </summary>
        private const int NODE_TIMEOUT_SECONDS = 60;

        #endregion

        #region Private Members

        /// <summary>
        /// WCF Mesh Service Host
        /// </summary>
        private ServiceHost serviceHost;

        /// <summary>
        /// WCF Channel Factory for IMeshNode Contract
        /// </summary>
        private ChannelFactory<IMeshNode> channelFactory;

        /// <summary>
        /// Heartbeat Timer
        /// </summary>
        private Timer heartbeatTimer;

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when the device reports a hit received.
        /// </summary>
        public event EventHandler<EventArgs> HeartbeatReceived = delegate { };

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the MeshNode singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public MeshNode Instance { get; private set; }

        /// <summary>
        /// Gets or the proxy.
        /// </summary>
        /// <value>The proxy.</value>
        public IMeshNode Proxy { get; private set; }

        /// <summary>
        /// Gets or sets the id of this server node.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            // Create the service host (a singleton instance of the concrete class MeshNode)
            this.Id = Guid.NewGuid();
            this.Instance = new MeshNode();
            this.serviceHost = new ServiceHost(this.Instance);
            this.serviceHost.Credentials.Peer.MeshPassword = MeshPassword;
            this.serviceHost.Open();

            // Create the client factory (IMeshNode)
            this.channelFactory = new ChannelFactory<IMeshNode>("IMeshNode");
            this.channelFactory.Credentials.Peer.MeshPassword = MeshPassword;
            this.Proxy = channelFactory.CreateChannel();
            PeerNode peerNode = ((IClientChannel)this.Proxy).GetProperty<PeerNode>();
            //peerNode.MessagePropagationFilter = new RemoteOnlyMessagePropagationFilter();

            // Create a timer to send out heartbeat signal
            this.heartbeatTimer = new Timer(
              this.SendHeartbeat,
              null,
              TimeSpan.Zero,
              TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.heartbeatTimer.Dispose();
            this.channelFactory.Close();
            this.serviceHost.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Stop();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendHeartbeat(object context)
        {
            System.Diagnostics.Trace.WriteLine("Sending heart beat signal from " + this.Id);
            this.Proxy.Heartbeat(this.Id);
        }

        #endregion
    }
}