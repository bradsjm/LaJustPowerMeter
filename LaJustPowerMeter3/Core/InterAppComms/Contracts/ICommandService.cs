namespace InterAppComms.Contracts
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public interface ICommandService: IBasicService
    {
        [OperationContract(IsOneWay=true)]
        void Execute(string command, params string[] param);
    }
}