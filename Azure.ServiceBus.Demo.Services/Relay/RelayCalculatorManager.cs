using System.ServiceModel;
using Azure.ServiceBus.Demo.Contracts.Relay;

namespace Azure.ServiceBus.Demo.Services.Relay
{
    [ServiceBehavior(Name = "RelayCalculatorManager", Namespace = "http://servicebus/demo/relay2")]
    public class RelayCalculatorManager : IRelayCalculatorService
    {
        public double Add(double a, double b)
        {
            return a + b;
        }

        public double Subtract(double a, double b)
        {
            return a - b;
        }

        public double Multiply(double a, double b)
        {
            return a*b;
        }

        public double Divide(double a, double b)
        {
            return a/b;
        }
    }
}
