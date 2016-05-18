using Azure.ServiceBus.Demo.Contracts;
using Microsoft.ServiceBus;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Azure.ServiceBus.Demo.Manager.Views
{
    public partial class RelayUserControl : UserControl
    {
        private SynchronizationContext _syncContext = null;

        public RelayUserControl()
        {
            InitializeComponent();
            this._syncContext = SynchronizationContext.Current;
        }

        private void BtnInvokeServiceClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LblReturnValue.Content = string.Empty;
                var input = this.TxtName.Text;

                Task.Run(() =>
                {
                    var factory = new ChannelFactory<IRelayTestService>(
                    new NetTcpRelayBinding(),
                    new EndpointAddress(
                        ServiceBusEnvironment.CreateServiceUri(
                            "sb", Common.AccessData.ServiceBusNamespace, "RelayTestService")));

                    factory.Endpoint.Behaviors.Add(
                        new TransportClientEndpointBehavior
                        {
                            TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                                Common.AccessData.SasRootKeyName,
                                Common.AccessData.SasRootKeyValue)
                        });

                    var proxy = factory.CreateChannel();
                    var result = proxy.SayHello(input);

                    factory.Close();

                    this._syncContext.Send(args =>
                    {
                        this.LblReturnValue.Content = result;
                    }, null);
                });                
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new Exception(ex.Message);
            }
        }
    }
}
