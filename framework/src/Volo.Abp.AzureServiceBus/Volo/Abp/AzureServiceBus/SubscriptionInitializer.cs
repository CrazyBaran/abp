using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus.Volo.Abp.AzureServiceBus
{
    public class SubscriptionInitializer : ISingletonDependency
    {
        protected IManagementClientPool ManagementClientPool { get; }

        protected AbpAzureServiceBusOptions Options { get; }

        public SubscriptionInitializer(IManagementClientPool managementClientPool,
            IOptions<AbpAzureServiceBusOptions> options)
        {
            ManagementClientPool = managementClientPool;
            Options = options.Value;
        }

        public async Task<SubscriptionDescription> EnsureSubscriptionExists(string topicName, string subscriptionName, string connectionName = null)
        {
            connectionName = connectionName
                ?? AzureServiceBusConnections.DefaultConnectionName;

            var managementClient = ManagementClientPool.Get(connectionName);

            if (await managementClient.SubscriptionExistsAsync(topicName, subscriptionName).ConfigureAwait(false))
            {
                return await managementClient.GetSubscriptionAsync(topicName, subscriptionName).ConfigureAwait(false);
            }

            try
            {
                return await managementClient.CreateSubscriptionAsync(topicName, subscriptionName).ConfigureAwait(false);
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                // most likely a race between two competing consumers - we should be able to get it now
                return await managementClient.GetSubscriptionAsync(topicName, subscriptionName).ConfigureAwait(false);
            }
        }
    }
}
