using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AzureServiceBus
{
    public class NamespaceManagerPool : INamespaceManagerPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        protected ConcurrentDictionary<string, NamespaceManager> NamespaceManagers { get; set; }
        public NamespaceManagerPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
            NamespaceManagers = new ConcurrentDictionary<string, NamespaceManager>();
        }

        public NamespaceManager Get(string connectionName = null)
        {
            connectionName = connectionName
                ?? AzureServiceBusConnections.DefaultConnectionName;

            return NamespaceManagers.GetOrAdd(connectionName,
                () => new NamespaceManager(Options
                .Connections
                .GetOrDefault(connectionName).ConnectionString)
                );
        }
    }
}