using Microsoft.ServiceBus;
using System;
using System.Diagnostics;
using System.ServiceModel;
using Azure.ServiceBus.Demo.Contracts.Relay;
using Azure.ServiceBus.Demo.Services.Relay;
using System.ServiceModel.Web;

namespace Azure.ServiceBus.Demo.RelayHosting
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost hostCalculatorManager = null;
            WebServiceHost hostImageDownloader = null;

            try
            {
                hostCalculatorManager = new ServiceHost(typeof(RelayCalculatorManager));
                hostImageDownloader = new WebServiceHost(typeof(ImageManager));

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

                #region Endpoint configuration - ImageManager

                var sasCredential = new TransportClientEndpointBehavior
                {
                    TokenProvider =
                        TokenProvider.CreateSharedAccessSignatureTokenProvider(
                            Common.AccessData.SasTestKeyName,
                            Common.AccessData.SasTestKeyValue)
                };

                hostImageDownloader.AddServiceEndpoint(
                    typeof(IImageService),
                    new WebHttpRelayBinding(
                        EndToEndWebHttpSecurityMode.Transport, 
                        RelayClientAuthenticationType.None),
                    ServiceBusEnvironment.CreateServiceUri(
                        "https", Common.AccessData.ServiceBusNamespace, "ImageService"));

                foreach (var endpoint in hostImageDownloader.Description.Endpoints)
                {
                    endpoint.Behaviors.Add(sasCredential);
                }

                #endregion

                Console.WriteLine("1) Start hosting the service(s)");
                Console.WriteLine();
                Console.WriteLine("......................................................");

                StartHostingService(hostCalculatorManager, false);
                StartHostingService(hostImageDownloader, true);

                Console.WriteLine();
                Console.WriteLine("2) press <enter> to exit...");
                Console.ReadKey();

                hostCalculatorManager.Close();
                hostImageDownloader.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();

                Abort(hostCalculatorManager);
                Abort(hostImageDownloader);

                throw;
            }
        }

        private static void Abort(ServiceHost host)
        {
            if (host != null && host.State == CommunicationState.Faulted)
            {
                host.Abort();
            }
        }

        private static void StartHostingService(ServiceHost host, bool isRest)
        {
            host.Open();

            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine();

                if (isRest)
                {
                    Console.WriteLine("   Address (copy the address into a browser to see the image):");
                    Console.WriteLine(string.Format("      {0}GetImage", endpoint.Address.Uri));
                }
                else
                {
                    Console.WriteLine("   Address:");
                    Console.WriteLine("      " + endpoint.Address.Uri);
                }
                
                Console.WriteLine();
                Console.WriteLine("   Binding:");
                Console.WriteLine("      " + endpoint.Binding.Name);
                Console.WriteLine();
                Console.WriteLine("   Contract:");
                Console.WriteLine("      " + endpoint.Contract.Name);
                Console.WriteLine();
                Console.WriteLine("......................................................");
            }
        }
    }
}
