namespace InterAppComms.Services
{
    using System;
    using System.ServiceModel;

    using InterAppComms.Contracts;
    using InterAppComms.Services;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BasicService : IBasicService
    {
        /// <summary>
        /// Occurs when [ping received].
        /// </summary>
        public event EventHandler<EventArgs> PingReceived = delegate { };

        #region IPingService Members

        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns>
        /// Always returns true
        /// </returns>
        public virtual bool Ping()
        {
            this.OnPingReceived();
            return true;
        }

        #endregion

        /// <summary>
        /// Called when [ping received].
        /// </summary>
        protected void OnPingReceived()
        {
            EventHandler<EventArgs> handler = this.PingReceived;
            handler(this, EventArgs.Empty);
        }

    }
}