namespace InterAppComms.Contracts
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public interface IBasicService
    {
        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns>Always returns true</returns>
        [OperationContract]
        bool Ping();
    }
}