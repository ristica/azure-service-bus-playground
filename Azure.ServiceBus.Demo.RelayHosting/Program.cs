using Azure.ServiceBus.Demo.Contracts;
using Azure.ServiceBus.Demo.Services;
using Microsoft.ServiceBus;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace Azure.ServiceBus.Demo.RelayHosting
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = null;

            try
            {
                host = new ServiceHost(typeof(RelayTestServiceManager));
                ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

                #region Endpoint configuration

                // localhost endpoint
                host.AddServiceEndpoint(
                    typeof(IRelayTestService),
                    new NetTcpBinding(),
                    "net.tcp://localhost:9876/RelayTestService"
                    );

                // azure endpoint
                host.AddServiceEndpoint(
                    typeof(IRelayTestService),
                    new NetTcpRelayBinding(),
                    ServiceBusEnvironment
                        .CreateServiceUri("sb", Common.AccessData.ServiceBusNamespace, "RelayTestService"))
                        .Behaviors.Add(new TransportClientEndpointBehavior
                        {
                            TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                                Common.AccessData.SasTestKeyName,
                                Common.AccessData.SasTestKeyValue)
                        });

                #endregion

                StartHostingService(host);

                Console.WriteLine();
                Console.WriteLine("2) press <enter> to close the app...");
                Console.ReadKey();

                host.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();

                if (host != null && host.State == CommunicationState.Faulted)
                {
                    host.Abort();
                }

                throw;
            }
        }
        private static void StartHostingService(ServiceHost host)
        {
            Console.WriteLine();
            Console.WriteLine("1) Start hosting the service(s)");
            Console.WriteLine();

            host.Open();

            Console.WriteLine("......................................................");
            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine();
                Console.Write("\tAddress:");
                Console.Write("  - " + endpoint.Address.Uri);
                Console.WriteLine();
                Console.Write("\tBinding:");
                Console.Write("  - " + endpoint.Binding.Name);
                Console.WriteLine();
                Console.Write("\tContract:");
                Console.Write(" - " + endpoint.Contract.Name);
                Console.WriteLine();
                Console.WriteLine("......................................................");
            }
        }
    }
}
