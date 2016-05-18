using System;
using System.ServiceModel;

namespace Azure.ServiceBus.Demo.Contracts
{
    [ServiceContract(Name = "IRelayTestService", Namespace ="http://servicebus.demo/relay")]
    public interface IRelayTestService : IDisposable
    {
        [OperationContract]
        string SayHello(string sender);
    }

    public interface IRelayTestChannel : IRelayTestService, IClientChannel { }
}
