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
    /// Abstract Communication Receiver Class
    /// </summary>
    public abstract class ReceiverBase<Service> : IDisposable where Service: BasicService, new()
    {
        #region Protected Variables

        /// <summary>
        /// Instance of the generic Service
        /// </summary>
        protected Service ServiceInstance = new Service();

        /// <summary>
        /// Host for service instance
        /// </summary>
        protected ServiceHost Host = null;

        /// <summary>
        /// Channel Name
        /// </summary>
        protected string ChannelName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the service instance.
        /// </summary>
        public Service Instance
        {
            get { return this.ServiceInstance; }
        }

        /// <summary>
        /// Gets the receiver channel name.
        /// </summary>
        public string Name
        {
            get { return this.ChannelName; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverBase"/> class.
        /// </summary>
        public ReceiverBase()  : this("Default") {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverBase"/> class.
        /// </summary>
        /// <param name="name">Channel Name</param>
        public ReceiverBase(string name)
        {
            System.Diagnostics.Debug.WriteLine("InterAppComms receiver using channel name \"" + name + "\"");
            this.ChannelName = name;
        }

        #endregion

        #region Public Operations

        /// <summary>
        /// Starts the receiver service.
        /// </summary>
        public virtual void StartService()
        {
            this.Host = this.CreateServiceHost(this.ChannelName);
        }

        /// <summary>
        /// Stops the receiver service.
        /// </summary>
        public virtual void StopService()
        {
            if (this.Host != null && this.Host.State != CommunicationState.Closed)
            {
                this.Host.Close();
            }
        }

        #endregion

        #region WCF Operations

        /// <summary>
        /// Hosts the service which will listen for communications
        /// </summary>
        protected abstract ServiceHost CreateServiceHost(string name);

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the default name of the channel.
        /// </summary>
        /// <returns></returns>
        static protected string GetAssemblyGuid()
        {
            var assembly = Assembly.GetExecutingAssembly();
            object[] objects = assembly.GetCustomAttributes(typeof(GuidAttribute), false);
            return ((GuidAttribute)objects.First()).Value;
        }

        #endregion

        #region IDisposable Members

        // A basic dispose.
        public void Dispose()
        {
            this.StopService();

            if (this.ServiceInstance != null)
                this.ServiceInstance = null;
        }

        #endregion
    }
}