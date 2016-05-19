using Microsoft.ServiceBus;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Azure.ServiceBus.Demo.Contracts.Relay;

namespace Azure.ServiceBus.Demo.Manager.Views
{
    public partial class RelayUserControl : UserControl
    {
        private SynchronizationContext _syncContext = null;

        public RelayUserControl()
        {
            InitializeComponent();

            // important!!!
            this._syncContext = SynchronizationContext.Current;
        }

        private void BtnInvokeServiceClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TxtFirstOperand.Text.Trim()))
            {
                MessageBox.Show("Please set first operand.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.TxtSecondOperand.Text.Trim()))
            {
                MessageBox.Show("Please set second operand.");
                return;
            }

            try
            {
                this.LblReturnValue.Content = string.Empty;

                var operation = this.GetChoosedOperation();
                if (string.IsNullOrEmpty(operation))
                {
                    MessageBox.Show("Please set an operation.");
                    return;
                }

                double a = 0;
                if (double.TryParse(this.TxtFirstOperand.Text, out a))
                {

                }
                else
                {

                }

                double b = 0;
                if (double.TryParse(this.TxtSecondOperand.Text, out b))
                {

                }
                else
                {

                }

                // allways do long running tasks in "background"
                // and do not tie ui to the task
                Task.Run(() =>
                {
                    var factory = new ChannelFactory<IRelayCalculatorService>(
                    new NetTcpRelayBinding(),
                    new EndpointAddress(
                        ServiceBusEnvironment.CreateServiceUri(
                            "sb", Common.AccessData.ServiceBusNamespace, "RelayCalculatorService")));

                    factory.Endpoint.Behaviors.Add(
                        new TransportClientEndpointBehavior
                        {
                            TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                                Common.AccessData.SasRootKeyName,
                                Common.AccessData.SasRootKeyValue)
                        });

                    var proxy = factory.CreateChannel();
                    double result = 0;
                    string op;
                    switch (operation)
                    {
                        case "add":
                            result = proxy.Add(a, b);
                            op = "+";
                            break;
                        case "subtract":
                            result = proxy.Subtract(a, b);
                            op = "-";
                            break;
                        case "divide":
                            result = proxy.Divide(a, b);
                            op = ":";
                            break;
                        default:
                            result = proxy.Multiply(a, b);
                            op = "*";
                            break;
                    }

                    factory.Close();

                    // marshalling background task to the ui thread
                    this._syncContext.Send(args =>
                    {
                        this.LblReturnValue.Content = $"{a} {op} {b} = {result}";
                    }, null);
                });                
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw new Exception(ex.Message);
            }
        }

        private string GetChoosedOperation()
        {
            if ((bool)this.RbAdd.IsChecked)
            {
                return "add";
            }
            if ((bool)this.RbSubtract.IsChecked)
            {
                return "subtract";
            }
            if ((bool)this.RbDivide.IsChecked)
            {
                return "divide";
            }
            if ((bool)this.RbMultiply.IsChecked)
            {
                return "multiply";
            }
            return string.Empty;
        }
    }
}
