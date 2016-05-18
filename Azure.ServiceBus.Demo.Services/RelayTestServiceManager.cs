using Azure.ServiceBus.Demo.Contracts;
using System;
using System.ServiceModel;

namespace Azure.ServiceBus.Demo.Services
{
    [ServiceBehavior(Name = "RelayTestServiceManager", Namespace = "http://servicebus.demo/relay")]
    public class RelayTestServiceManager : IRelayTestService
    {
        public string SayHello(string sender)
        {
            Console.WriteLine();
            Console.WriteLine("\tRelayTestServiceManager invoked with parameter: " + sender);
            Console.WriteLine();
            return string.Format("Hello {0}!", sender);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free your resurces
                // ...
            }
        }
    }
}
