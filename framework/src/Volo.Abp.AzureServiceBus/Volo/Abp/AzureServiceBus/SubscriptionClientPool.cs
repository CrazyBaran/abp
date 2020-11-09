using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk#reusing-factories-and-clients
    public class SubscriptionClientPool : ISubscriptionClientPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected IServiceBusConnectionPool ServiceBusConnectionPool { get; }

        protected ConcurrentDictionary<ValueTuple<string, string, string>, ISubscriptionClient> Subscriptions { get; set; }

        private bool _isDisposed;

        public SubscriptionClientPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
        }

        public ISubscriptionClient Get(string topicName, string subscriptionName, string connectionName = null)
        {
            connectionName = connectionName
                 ?? AzureServiceBusConnections.DefaultConnectionName;

            return Subscriptions.GetOrAdd(
                new ValueTuple<string, string, string>(connectionName, topicName, subscriptionName),
                () => new SubscriptionClient(ServiceBusConnectionPool.Get(connectionName), 
                topicName, subscriptionName, ReceiveMode.PeekLock, null));
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Subscriptions.Clear();
        }
    }
}
