using System;
using System.ServiceModel;
using Azure.ServiceBus.Demo.Contracts.Relay;

namespace Azure.ServiceBus.Demo.Services.Relay
{
    [ServiceBehavior(Name = "RelayCalculatorManager", Namespace = "http://servicebus/demo/relay")]
    public class RelayCalculatorManager : IRelayCalculatorService
    {
        public double Add(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("\tRelayCalculatorManager.Add() invoked with parameter: {0} and {1}", a, b));
            Console.WriteLine();
            return a + b;
        }

        public double Subtract(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("\tRelayCalculatorManager.Subtract() invoked with parameter: {0} and {1}", a, b));
            Console.WriteLine();
            return a - b;
        }

        public double Multiply(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("\tRelayCalculatorManager.Multiply() invoked with parameter: {0} and {1}", a, b));
            Console.WriteLine();
            return a*b;
        }

        public double Divide(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("\tRelayCalculatorManager.Divide() invoked with parameter: {0} and {1}", a, b));
            Console.WriteLine();
            return a/b;
        }
    }
}
