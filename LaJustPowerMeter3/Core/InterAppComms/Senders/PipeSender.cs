namespace InterAppComms
{
    using System;
    using System.ServiceModel;

    using InterAppComms.Contracts;
    using InterAppComms.Services;
    using System.Diagnostics;

    /// <summary>
    /// Named Pipe Sender Communication Class
    /// </summary>
    public class PipeSender<T> : SenderBase<T> where T : IBasicService
    {
        private const string PipeBaseUri = "net.pipe://localhost/";

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="assemblyGuid">The assembly GUID.</param>
        /// <returns></returns>
        protected override ChannelFactory<T> CreateChannel(string assemblyGuid, string name)
        {
            var endPointAddress = new EndpointAddress(PipeBaseUri + assemblyGuid + "/" + name);
            var channelFactory = new ChannelFactory<T>(new NetNamedPipeBinding(), endPointAddress);
            Trace.WriteLine("Creating WCF Service Sender Host at " + endPointAddress);
            channelFactory.Open();
            return channelFactory;
        }
    }
}
