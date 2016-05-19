using Microsoft.ServiceBus;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;
using Azure.ServiceBus.Demo.Manager.Dialogs;

namespace Azure.ServiceBus.Demo.Manager.Views
{
    public partial class TopicUserControl : UserControl
    {
        #region Fields

        private static ObservableCollection<string> Topics = new ObservableCollection<string>();
        private static ObservableCollection<string> Messages = new ObservableCollection<string>();
        private SynchronizationContext _sync;
        private SqlFilter _greaterThenFilter;
        private SqlFilter _lessThenFilter;

        #endregion

        #region C-Tor

        public TopicUserControl()
        {
            InitializeComponent();
            this.CbTopics.ItemsSource = Topics;
            this.LbMessages.ItemsSource = Messages;
            this._sync = SynchronizationContext.Current;
            this.LoadTopics();
            this.CreateFilter();
        }

        #endregion

        #region Events

        private void BtnShowSubscribersClicked(object sender, RoutedEventArgs e)
        {
            var selectedTopic = this.CbTopics.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTopic))
            {
                MessageBox.Show("Please choose a topic at first.");
                return;
            }

            var window = new Subscribers(selectedTopic);
            window.LblTopic.Content = string.Format("Current Topic - '{0}'", selectedTopic);
            window.Show();
        }

        private void BtnSendMessageClicked(object sender, RoutedEventArgs e)
        {
            var selectedTopic = this.CbTopics.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(selectedTopic))
            {
                MessageBox.Show("Please choose a topic.");
                return;
            }

            var message = this.TxtMessage.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please put a text to send.");
                return;
            }

            var client = this.GetFactory().CreateTopicClient(selectedTopic);

            var m = new BrokeredMessage(message);

            int input;
            if (int.TryParse(message, out input))
            {
                
            }
            else
            {
                MessageBox.Show("Only 'integer' as message allowed.");
                return;
            }

            // app specific property to filter subscription on
            m.Properties["Input"] = input;

            client.Send(m);

            Messages.Add(message);
            this.TxtMessage.Text = string.Empty;
        }

        private void BtnDeleteTopicClicked(object sender, RoutedEventArgs e)
        {
            var selectedTopic = this.CbTopics.SelectedItem as string;
            if (selectedTopic == null)
            {
                MessageBox.Show("Please chosse the topic to delete.");
                return;
            }

            this.GetNamespaceManager().DeleteTopic(selectedTopic);

            if (Topics.Contains(selectedTopic))
            {
                Topics.Remove(selectedTopic);
            }

            Messages.Clear();

            this.ShowPrompt("deleted", selectedTopic);
            this.TxtMessage.Text = string.Empty;
            Messages.Clear();
        }

        private void BtnCreateTopicClicked(object sender, RoutedEventArgs e)
        {
            var topic = this.TxtTopicName.Text;
            if (string.IsNullOrWhiteSpace(topic))
            {
                MessageBox.Show("Please give the topic a name.");
                return;
            }

            var td = new TopicDescription(topic)
            {
                MaxSizeInMegabytes = 5120,
                DefaultMessageTimeToLive = new TimeSpan(0, 5, 0)
            };

            var namespaceManager = this.GetNamespaceManager();

            if (namespaceManager.QueueExists(topic))
            {
                MessageBox.Show(string.Format("Topic '{0}' exists allready.", topic));
                return;
            }

            namespaceManager.CreateTopic(td);

            this.CreateSubscriptions(topic, namespaceManager);

            if (!Topics.Contains(topic))
            {
                Topics.Add(topic);
            }

            this.TxtTopicName.Text = string.Empty;

            this.ShowPrompt("created", topic);
        }

        private void CbTopicsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTopic = this.CbTopics.SelectedItem as string;
            this.TxtMessage.Text = string.Empty;
            Messages.Clear();
            this.CreateSubscriptions(selectedTopic);
        }

        #endregion

        #region Helpers

        private void CreateFilter()
        {
            this._greaterThenFilter = new SqlFilter("Input > 100");
            this._lessThenFilter = new SqlFilter("Input < 100");
        }

        private void CreateSubscriptions(string topic, NamespaceManager namespaceManager = null)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return;
            }

            if (namespaceManager == null)
            {
                namespaceManager = this.GetNamespaceManager();
            }

            if (!namespaceManager.SubscriptionExists(topic, "MatchAllSubscription"))
            {
                namespaceManager.CreateSubscription(topic, "MatchAllSubscription");
            }

            if ((bool)CheckLessThen.IsChecked && !namespaceManager.SubscriptionExists(topic, "LessThenSubscription"))
            {
                namespaceManager.CreateSubscription(topic, "LessThenSubscription", this._lessThenFilter);
            }

            if ((bool)CheckGreaterThen.IsChecked && !namespaceManager.SubscriptionExists(topic, "GreaterThenSubscription"))
            {
                namespaceManager.CreateSubscription(topic, "GreaterThenSubscription", this._greaterThenFilter);
            }
        }

        private MessagingFactory GetFactory()
        {
            return MessagingFactory.Create(
                ServiceBusEnvironment.CreateServiceUri(
                    "sb",
                    Common.AccessData.ServiceBusNamespace,
                    string.Empty),
                this.GetCredentials());
        }

        private void ShowPrompt(string message, string topic)
        {
            this.LblMessage.Content = string.Format("Topic '{0}' {1}...", topic, message);
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                this._sync.Send(arg =>
                {
                    this.LblMessage.Content = string.Empty;
                }, null);
            });
        }

        private void LoadTopics()
        {
            if (Topics.Any())
            {
                return;
            }

            var manager = this.GetNamespaceManager();
            var topics = manager.GetTopics();

            foreach (var t in topics)
            {
                Topics.Add(t.Path);
            }
        }

        private TokenProvider GetCredentials()
        {
            return TokenProvider
                .CreateSharedAccessSignatureTokenProvider(
                    Common.AccessData.SasTestKeyName,
                    Common.AccessData.SasTestKeyValue);
        }

        private NamespaceManager GetNamespaceManager()
        {
            return new NamespaceManager(
                ServiceBusEnvironment.CreateServiceUri(
                    "sb",
                    Common.AccessData.ServiceBusNamespace,
                    string.Empty),
                this.GetCredentials());
        }

        #endregion
    }
}
