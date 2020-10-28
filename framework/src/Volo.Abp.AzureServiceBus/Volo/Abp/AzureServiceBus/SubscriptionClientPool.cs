using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus.Volo.Abp.AzureServiceBus
{
    public class SubscriptionClientPool : ISubscriptionClientPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected ConcurrentDictionary<ValueTuple<string, string>, ISubscriptionClient> Subscriptions { get; set; }

        private bool _isDisposed;

        public SubscriptionClientPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
        }

        public ISubscriptionClient Get(string topic, string connectionName = null)
        {
            connectionName = connectionName
                 ?? AzureServiceBusConnections.DefaultConnectionName;

            return Subscriptions.GetOrAdd(
                new ValueTuple<string, string>(connectionName, topic),
                () => new SubscriptionClient("", topic, "Default"));
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
