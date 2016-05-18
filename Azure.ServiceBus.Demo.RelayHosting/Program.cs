using Microsoft.ServiceBus;
using System;
using System.Diagnostics;
using System.ServiceModel;
using Azure.ServiceBus.Demo.Contracts.Relay;
using Azure.ServiceBus.Demo.Services.Relay;

namespace Azure.ServiceBus.Demo.RelayHosting
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost hostCalculatorManager = null;

            try
            {
                hostCalculatorManager = new ServiceHost(typeof(RelayCalculatorManager));

                ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

                #region Endpoint configuration - CalculatorManager

                hostCalculatorManager.AddServiceEndpoint(
                    typeof(IRelayCalculatorService),
                    new NetTcpRelayBinding(),
                    ServiceBusEnvironment
                        .CreateServiceUri("sb", Common.AccessData.ServiceBusNamespace, "RelayCalculatorService"))
                        .Behaviors.Add(new TransportClientEndpointBehavior
                        {
                            TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                                Common.AccessData.SasTestKeyName,
                                Common.AccessData.SasTestKeyValue)
                        });

                #endregion

                Console.WriteLine("1) Start hosting the service(s)");
                Console.WriteLine();
                Console.WriteLine("......................................................");

                StartHostingService(hostCalculatorManager);

                Console.WriteLine();
                Console.WriteLine("2) press <enter> to close the app...");
                Console.ReadKey();

                hostCalculatorManager.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();

                if (hostCalculatorManager != null && hostCalculatorManager.State == CommunicationState.Faulted)
                {
                    hostCalculatorManager.Abort();
                }

                throw;
            }
        }
        private static void StartHostingService(ServiceHost host)
        {
            host.Open();

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
