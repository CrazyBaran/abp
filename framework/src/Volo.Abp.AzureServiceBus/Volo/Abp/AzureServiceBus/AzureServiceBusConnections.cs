using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Volo.Abp.AzureServiceBus
{
    [Serializable]
    public class AzureServiceBusConnections : Dictionary<string, string>
    {
        public const string DefaultConnectionName = "Default";

        [NotNull]
        public string Default
        {
            get => this[DefaultConnectionName];
            set => this[DefaultConnectionName] = Check.NotNull(value, nameof(value));
        }

        public string GetOrDefault(string connectionName)
        {
            if (TryGetValue(connectionName, out var connectionFactory))
            {
                return connectionFactory;
            }

            return Default;
        }
    }
}
