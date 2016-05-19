using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Azure.ServiceBus.Demo.Manager.Views
{
    public partial class QueueUserControl : UserControl
    {
        #region Fields

        private static ObservableCollection<string> Queues = new ObservableCollection<string>();
        private static ObservableCollection<string> Messages = new ObservableCollection<string>();
        private SynchronizationContext _sync;
        private static QueueClient _client;

        #endregion

        #region C-Tor

        public QueueUserControl()
        {
            InitializeComponent();
            this.CbQueues.ItemsSource = Queues;
            this.LbMessages.ItemsSource = Messages;
            this.Loaded += ViewLoaded;
            this._sync = SynchronizationContext.Current;
        }

        #endregion

        #region Events

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            if (Queues.Any())
            {
                return;
            }

            var manager = this.GetNamespaceManager();
            var queues = manager.GetQueues();

            foreach ( var q in queues)
            {
                Queues.Add(q.Path);
            }
        }

        private void BtnDeleteQueueClicked(object sender, RoutedEventArgs e)
        {
            var selectedQueue = this.CbQueues.SelectedItem as string;
            if (selectedQueue == null)
            {
                MessageBox.Show("Please chosse the queue to delete.");
                return;
            }

            if (_client != null && !_client.IsClosed)
            {
                _client.Close();
            }

            this.GetNamespaceManager().DeleteQueue(selectedQueue);

            if (Queues.Contains(selectedQueue))
            {
                Queues.Remove(selectedQueue);
            }

            Messages.Clear();

            this.ShowPrompt("deleted", selectedQueue);
        }

        private void BtnCreateQueueClicked(object sender, RoutedEventArgs e)
        {
            var queue = this.TxtQueueName.Text;
            if (string.IsNullOrWhiteSpace(queue))
            {
                MessageBox.Show("Please give the queue a name.");
                return;
            }

            var qd = new QueueDescription(queue)
            {
                MaxSizeInMegabytes = 5120,
                DefaultMessageTimeToLive = new TimeSpan(0, 5, 0)
            };

            var namespaceManager = this.GetNamespaceManager();

            if (namespaceManager.QueueExists(queue))
            {
                MessageBox.Show(string.Format("Queue '{0}' exists allready.", queue));
                return;
            }
            namespaceManager.CreateQueue(queue);

            if (!Queues.Contains(queue))
            {
                Queues.Add(queue);
            }
            this.TxtQueueName.Text = string.Empty;

            this.ShowPrompt("created", queue);
        }

        private void BtnSendMessageClicked(object sender, RoutedEventArgs e)
        {
            var selectedQueue = this.CbQueues.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedQueue))
            {
                MessageBox.Show("Please choose a queue.");
                return;
            }

            var message = this.TxtMessage.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please put a text to send.");
                return;
            }

            var connectionString = CloudConfigurationManager.GetSetting(Common.AccessData.ServiceBusConfigConnectionStringName);
            var client = QueueClient.CreateFromConnectionString(connectionString, selectedQueue);

            var m = new BrokeredMessage(message);
            client.Send(m);
        }

        private void CbQueuesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedQueue = this.CbQueues.SelectedItem as string;
            Messages.Clear();
            ListenToTheQueue(selectedQueue);
        }

        #endregion

        #region Helpers

        private NamespaceManager GetNamespaceManager()
        {
            var connectionString = CloudConfigurationManager.GetSetting(Common.AccessData.ServiceBusConfigConnectionStringName);
            return NamespaceManager.CreateFromConnectionString(connectionString);
        }

        private void ListenToTheQueue(string queue)
        {
            if (string.IsNullOrWhiteSpace(queue))
            {
                return;
            }

            var connectionString = CloudConfigurationManager.GetSetting(Common.AccessData.ServiceBusConfigConnectionStringName);
            _client = QueueClient.CreateFromConnectionString(connectionString, queue);
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromSeconds(10),
                MaxConcurrentCalls = 1
            };

            Task.Run(() =>
            {
                _client.OnMessage((message) =>
                {
                    try
                    {
                        this._sync.Send(arg =>
                        {
                            var m = message.GetBody<string>();

                            Debug.Print("\tBody:          " + m);
                            Debug.Print("\tMessageID:     " + message.MessageId);
                            Debug.Print("");

                            Messages.Add(m);

                            this.TxtMessage.Text = string.Empty;

                        }, null);

                        // Remove message from queue.
                        message.Complete();
                    }
                    catch (Exception)
                    {
                        // Indicates a problem, unlock message in queue.
                        message.Abandon();
                    }
                }, options);
            });
        }

        private void ShowPrompt(string message, string queue)
        {
            this.LblMessage.Content = string.Format("Queue '{0}' {1}...", queue, message);
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                this._sync.Send(arg =>
                {
                    this.LblMessage.Content = string.Empty;
                }, null);
            });
        }

        #endregion
    }
}
