using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    public class ManagementClientPool : IManagementClientPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected ConcurrentDictionary<string, ManagementClient> ManagementClients { get; set; }

        public ManagementClientPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
            ManagementClients = new ConcurrentDictionary<string, ManagementClient>();
        }

        public ManagementClient Get(string connectionName = null)
        {
            connectionName = connectionName
                ?? AzureServiceBusConnections.DefaultConnectionName;

            return ManagementClients.GetOrAdd(connectionName,
                () => new ManagementClient(Options
                        .Connections
                        .GetOrDefault(connectionName).ConnectionString)
    );
        }
    }
}
