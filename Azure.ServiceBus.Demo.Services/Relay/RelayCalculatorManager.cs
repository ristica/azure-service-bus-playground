using System;
using System.ServiceModel;
using Azure.ServiceBus.Demo.Contracts.Relay;

namespace Azure.ServiceBus.Demo.Services.Relay
{
    [ServiceBehavior(Name = "RelayCalculatorManager", Namespace = "http://servicebus/demo/relay2")]
    public class RelayCalculatorManager : IRelayCalculatorService
    {
        public double Add(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine("\tRelayCalculatorManager.Add() invoked with parameter: " + a + " and " + b);
            Console.WriteLine();
            return a + b;
        }

        public double Subtract(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine("\tRelayCalculatorManager.Subtract() invoked with parameter: " + a + " and " + b);
            Console.WriteLine();
            return a - b;
        }

        public double Multiply(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine("\tRelayCalculatorManager.Multiply() invoked with parameter: " + a + " and " + b);
            Console.WriteLine();
            return a*b;
        }

        public double Divide(double a, double b)
        {
            Console.WriteLine();
            Console.WriteLine("\tRelayCalculatorManager.Divide() invoked with parameter: " + a + " and " + b);
            Console.WriteLine();
            return a/b;
        }
    }
}
