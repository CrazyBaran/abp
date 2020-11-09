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
    public class ServiceBusConnectionPool : IServiceBusConnectionPool, ISingletonDependency
    {
        protected AbpAzureServiceBusOptions Options { get; }

        private bool _isDisposed;

        protected ConcurrentDictionary<string, ServiceBusConnection> Connections { get; set; }
        public ServiceBusConnectionPool(IOptions<AbpAzureServiceBusOptions> options)
        {
            Options = options.Value;
            Connections = new ConcurrentDictionary<string, ServiceBusConnection>();
        }

        public ServiceBusConnection Get(string connectionName = null)
        {
            connectionName = connectionName
                 ?? AzureServiceBusConnections.DefaultConnectionName;

            return Connections.GetOrAdd(connectionName,
                () => new ServiceBusConnection(Options
                .Connections
                .GetOrDefault(connectionName).ConnectionString)
                );
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            Connections.Clear();
        }
    }
}
