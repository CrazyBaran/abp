using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    public class TopicClientPool : ITopicClientPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected ConcurrentDictionary<string, ITopicClient> Topics { get; set; }

        private bool _isDisposed;

        public TopicClientPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
            Topics = new ConcurrentDictionary<string, ITopicClient>();
        }

        public ITopicClient Get(string topic, string connectionName = null)
        {
            connectionName = connectionName
                             ?? AzureServiceBusConnections.DefaultConnectionName;
            
            return Topics.GetOrAdd(
                $"{connectionName}@{topic}",
                () => new TopicClient("", topic));
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Topics.Clear();
        }
    }
}
