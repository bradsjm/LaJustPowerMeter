namespace InterAppComms
{
    using System;
    using System.Linq;
    using System.ServiceModel;

    using InterAppComms.Contracts;
    using InterAppComms.Services;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Named Pipe Sender Communication Class
    /// </summary>
    public abstract class SenderBase<T> where T : IBasicService
    {
        #region Protected Members

        /// <summary>
        /// Name of Channel
        /// </summary>
        protected string ChannelName;

        /// <summary>
        /// The channel factory instance
        /// </summary>
        protected ChannelFactory<T> ChannelFactory;

        /// <summary>
        /// WCF Proxy Instance
        /// </summary>
        protected T Proxy;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the proxy service instance.
        /// </summary>
        public T Instance
        {
            get { return this.Proxy; }
        }

        /// <summary>
        /// Gets the channel.
        /// </summary>
        public ChannelFactory<T> Channel 
        {
            get { return this.ChannelFactory; } 
        }

        /// <summary>
        /// Gets the sender channel name.
        /// </summary>
        public string Name
        {
            get { return this.ChannelName; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderBase"/> class.
        /// </summary>
        public SenderBase()  : this("Default") {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderBase"/> class.
        /// </summary>
        /// <param name="name">Channel Name</param>
        public SenderBase(string name)
        {
            System.Diagnostics.Debug.WriteLine("InterAppComms sender using channel name \"" + name + "\"");
            this.ChannelName = name;
            this.ChannelFactory = this.CreateChannel(GetAssemblyGuid(), name);
            this.Proxy = this.CreateProxy();
        }

        #endregion

        protected abstract ChannelFactory<T> CreateChannel(string assemblyGuid, string name);

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateProxy()
        {
            return this.ChannelFactory.CreateChannel();
        }

        #region Private Methods

        /// <summary>
        /// Gets the default name of the channel.
        /// </summary>
        /// <returns></returns>
        static private string GetAssemblyGuid()
        {
            var assembly = Assembly.GetExecutingAssembly();
            object[] objects = assembly.GetCustomAttributes(typeof(GuidAttribute), false);
            return ((GuidAttribute)objects.First()).Value;
        }

        #endregion

    }
}
