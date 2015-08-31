namespace Network
{
    using System;
    using System.ServiceModel;
    using LaJust.EIDSS.Communications.Entities;

    /// <summary>
    /// WCF service contract for Worker communication.
    /// </summary>
    [ServiceContract]
    public interface IMeshNode
    {
        [OperationContract(IsOneWay = true)]
        void Heartbeat(Guid nodeId);

        [OperationContract(IsOneWay = true)]
        void DeviceRegistered(DeviceRegistrationEventArgs args);
    }
}
