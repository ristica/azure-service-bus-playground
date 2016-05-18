using Azure.ServiceBus.Demo.Contracts;
using Azure.ServiceBus.Demo.Services;
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
            ServiceHost hostTestManager = null;
            ServiceHost hostCalculatorManager = null;

            try
            {
                hostTestManager = new ServiceHost(typeof(RelayTestManager));
                hostCalculatorManager = new ServiceHost(typeof(RelayCalculatorManager));

                ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

                #region Endpoint configuration - TestManager

                // localhost endpoint
                hostTestManager.AddServiceEndpoint(
                    typeof(IRelayTestService),
                    new NetTcpBinding(),
                    "net.tcp://localhost:9876/RelayTestService"
                    );

                // azure endpoint
                hostTestManager.AddServiceEndpoint(
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

                StartHostingService(hostTestManager);
                StartHostingService(hostCalculatorManager);

                Console.WriteLine();
                Console.WriteLine("2) press <enter> to close the app...");
                Console.ReadKey();

                hostTestManager.Close();
                hostCalculatorManager.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();

                if (hostTestManager != null && hostTestManager.State == CommunicationState.Faulted)
                {
                    hostTestManager.Abort();
                }
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
