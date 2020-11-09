using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk#reusing-factories-and-clients
    public class TopicClientPool : ITopicClientPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected IServiceBusConnectionPool ServiceBusConnectionPool { get; }

        protected TopicInitializer TopicInitialization { get; }

        protected ConcurrentDictionary<ValueTuple<string, string>, ITopicClient> Topics { get; set; }

        private bool _isDisposed;

        public TopicClientPool(IOptions<AbpAzureServiceBusOptions> options,
            IServiceBusConnectionPool serviceBusConnectionPool,
            TopicInitializer topicInitializaton)
        {
            Options = options.Value;
            ServiceBusConnectionPool = serviceBusConnectionPool;
            TopicInitialization = topicInitializaton;
            Topics = new ConcurrentDictionary<ValueTuple<string, string>, ITopicClient>();
        }

        public async Task<ITopicClient> Get(string topic, string connectionName = null)
        {
            connectionName = connectionName
                             ?? AzureServiceBusConnections.DefaultConnectionName;

            var option = Options.Connections.GetOrDefault(connectionName);
            if (option.IsAutomaticlyCreateTopicAndSubscriptions && Topics.Any(x=> x.Key != (connectionName, topic)))
            {
                await TopicInitialization.EnsureTopicExists(topic, connectionName);
            }

            return Topics.GetOrAdd(
                (connectionName, topic), 
                () => new TopicClient(ServiceBusConnectionPool.Get(connectionName), topic, RetryPolicy.Default));
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
