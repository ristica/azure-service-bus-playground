using System;
using MahApps.Metro.Controls;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Collections.ObjectModel;
using System.Threading;

namespace Azure.ServiceBus.Demo.Manager.Dialogs
{
    public partial class Subscribers : MetroWindow
    {
        private ObservableCollection<string> MatchAllMessages = new ObservableCollection<string>();
        private ObservableCollection<string> LessThenMessages = new ObservableCollection<string>();
        private ObservableCollection<string> GreaterThenMessages = new ObservableCollection<string>();
        private SynchronizationContext _sync;
        public string _currentTopic;

        public Subscribers(string topic)
        {
            InitializeComponent();
            this._sync = SynchronizationContext.Current;
            this._currentTopic = topic;

            this.LbMatchAll.ItemsSource = this.MatchAllMessages;
            this.LbLessThen.ItemsSource = this.LessThenMessages;
            this.LbGreaterThen.ItemsSource = this.GreaterThenMessages;

            var factory = this.GetFactory();
            var namespaceManager = this.GetNamespaceManager();

            this.SetMatchAllSubscription(factory, namespaceManager);
            this.SetLessThenSubscription(factory, namespaceManager);
            this.SetGreaterThenSubscription(factory, namespaceManager);
        }

        private void SetMatchAllSubscription(MessagingFactory factory, NamespaceManager namespaceManager)
        {
            var client = factory.CreateSubscriptionClient(this._currentTopic, "MatchAllSubscription");
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };
            client.OnMessage((message) =>
            {
                try
                {
                    var m = message.GetBody<string>();
                    this._sync.Send(arg =>
                    {
                        MatchAllMessages.Add(m);
                    }, null);
                }
                catch(Exception ex)
                {
                    message.Abandon();
                }
            }, options);
        }

        private void SetLessThenSubscription(MessagingFactory factory, NamespaceManager namespaceManager)
        {
            if (!namespaceManager.SubscriptionExists(this._currentTopic, "LessThenSubscription"))
            {
                return;
            }

            var client = factory.CreateSubscriptionClient(this._currentTopic, "LessThenSubscription");
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };
            client.OnMessage((message) =>
            {
                try
                {
                    var m = message.GetBody<string>();
                    this._sync.Send(arg =>
                    {
                        LessThenMessages.Add(m);
                    }, null);
                }
                catch (Exception ex)
                {
                    message.Abandon();
                }
            }, options);
        }

        private void SetGreaterThenSubscription(MessagingFactory factory, NamespaceManager namespaceManager)
        {
            if (!namespaceManager.SubscriptionExists(this._currentTopic, "GreaterThenSubscription"))
            {
                return;
            }

            var client = factory.CreateSubscriptionClient(this._currentTopic, "GreaterThenSubscription");
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };
            client.OnMessage((message) =>
            {
                try
                {
                    var m = message.GetBody<string>();
                    this._sync.Send(arg =>
                    {
                        GreaterThenMessages.Add(m);
                    }, null);
                }
                catch (Exception ex)
                {
                    message.Abandon();
                }
            }, options);
        }

        #region Helpers

        private MessagingFactory GetFactory()
        {
            return MessagingFactory.Create(
                ServiceBusEnvironment.CreateServiceUri(
                    "sb",
                    Common.AccessData.ServiceBusNamespace,
                    string.Empty),
                this.GetCredentials());
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
