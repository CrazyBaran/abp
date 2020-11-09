using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Volo.Abp.AzureServiceBus
{
    [Serializable]
    public class AzureServiceBusConnections : Dictionary<string, AzureServiceBusConnection>
    {
        public const string DefaultConnectionName = "Default";

        [NotNull]
        public AzureServiceBusConnection Default
        {
            get => this[DefaultConnectionName];
            set => this[DefaultConnectionName] = Check.NotNull(value, nameof(value));
        }

        public AzureServiceBusConnection GetOrDefault(string connectionName)
        {
            if (TryGetValue(connectionName, out var connection))
            {
                return connection;
            }

            return Default;
        }
    }
}
