using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Azure.ServiceBus.Demo.Manager.Views
{
    public partial class QueueUserControl : UserControl
    {
        private static ObservableCollection<string> Queues = new ObservableCollection<string>();
         
        public QueueUserControl()
        {
            InitializeComponent();
            this.CbQueues.ItemsSource = Queues;
        }

        private void BtnDeleteQueueClicked(object sender, RoutedEventArgs e)
        {
            var selectedQueue = this.CbQueues.SelectedItem;
            if (selectedQueue == null)
            {
                MessageBox.Show("Please chosse the queue to delete.");
                return;
            }
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

            var connectionString = CloudConfigurationManager.GetSetting(Common.AccessData.ServiceBusConfigConnectionStringName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

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
        }

        private void BtnSendMessageClicked(object sender, RoutedEventArgs e)
        {
            var message = this.TxtMessage.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please put a text to send.");
                return;
            }
        }
    }
}
