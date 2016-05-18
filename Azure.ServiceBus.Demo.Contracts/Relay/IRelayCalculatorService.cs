using System.ServiceModel;

namespace Azure.ServiceBus.Demo.Contracts.Relay
{
    [ServiceContract(Name = "IRelayCalculatorService", Namespace = "http://servicebus/demo/relay2")]
    public interface IRelayCalculatorService
    {
        [OperationContract]
        double Add(double a, double b);

        [OperationContract]
        double Subtract(double a, double b);

        [OperationContract]
        double Multiply(double a, double b);

        [OperationContract]
        double Divide(double a, double b);
    }

    public interface IRelayCalculatorChannel : IRelayCalculatorService, IClientChannel { }
}
