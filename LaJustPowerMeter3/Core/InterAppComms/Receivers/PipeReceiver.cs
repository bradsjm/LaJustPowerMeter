namespace InterAppComms
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using InterAppComms.Contracts;
    using InterAppComms.Services;

    /// <summary>
    /// Named Pipe Communication Receiver Class
    /// </summary>
    public class PipeReceiver<Contract, Service> : ReceiverBase<Service> 
        where Contract: IBasicService
        where Service: BasicService, new()
    {
        private const string PipeBaseUri = "net.pipe://localhost/";

        /// <summary>
        /// Hosts the service which will listen for communications
        /// from the other side of the pipe.
        /// </summary>
        protected override ServiceHost CreateServiceHost(string name)
        {
            var host = new ServiceHost(ServiceInstance, new Uri(PipeBaseUri + GetAssemblyGuid()));
            host.AddServiceEndpoint(typeof(Contract), new NetNamedPipeBinding(), name);
            Trace.WriteLine("Creating WCF Service Receiver Host at " + PipeBaseUri + GetAssemblyGuid() + "/" + name);
            host.Open();
            return host;
        }
    }
}