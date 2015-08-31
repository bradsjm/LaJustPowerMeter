namespace InterAppComms
{
    using System;
    using System.ServiceModel;
    using InterAppComms.Contracts;
    using InterAppComms.Services;
    using System.Diagnostics;

    /// <summary>
    /// Mesh Communication Receiver Class
    /// </summary>
    public class MeshReceiver<Contract, Service> : ReceiverBase<Service> 
        where Contract: IBasicService
        where Service: BasicService, new()
    {
        /// <summary>
        /// Mesh URI
        /// </summary>
        const string MeshBaseUri = "net.p2p://";

        /// <summary>
        /// Private mesh password
        /// </summary>
        const string MeshPassword = "cSwPC9EQBUyizs7c993HEvTZ109e57e0";

        /// <summary>
        /// Hosts the service which will listen for communications
        /// from the other side of the pipe.
        /// </summary>
        protected override ServiceHost CreateServiceHost(string name)
        {
            var host = new ServiceHost(ServiceInstance, new Uri(MeshBaseUri + GetAssemblyGuid()));
            host.AddServiceEndpoint(typeof(Contract), new NetPeerTcpBinding(), name);
            host.Credentials.Peer.MeshPassword = MeshPassword;
            Trace.WriteLine("Creating WCF Service Receiver Host at " + MeshBaseUri + GetAssemblyGuid() + "/" + name);
            host.Open();
            return host;
        }
    }
}