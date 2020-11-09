using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    public class TopicInitializer : ISingletonDependency
    {
        protected IManagementClientPool ManagementClientPool { get; }

        protected AbpAzureServiceBusOptions Options { get; }

        public TopicInitializer(IManagementClientPool managementClientPool,
            IOptions<AbpAzureServiceBusOptions> options)
        {
            ManagementClientPool = managementClientPool;
            Options = options.Value;
        }

        public async Task<TopicDescription> EnsureTopicExists(string topic, string connectionName = null)
        {
            connectionName = connectionName
                 ?? AzureServiceBusConnections.DefaultConnectionName;

            var managementClient = ManagementClientPool.Get(connectionName);
            if (await managementClient.TopicExistsAsync(topic).ConfigureAwait(false))
            {
                return await managementClient.GetTopicAsync(topic).ConfigureAwait(false);
            }

            try
            {
                return await managementClient.CreateTopicAsync(topic).ConfigureAwait(false);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                // most likely a race between two clients trying to create the same topic - we should be able to get it now
                return await managementClient.GetTopicAsync(topic).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"Could not create topic '{topic}'", exception);
            }
        }
    }
}
