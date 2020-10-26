using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AzureServiceBus
{
    [Serializable]
    public class AzureServiceBusConnections
    {
        public const string DefaultConnectionName = "Default";
        public string QueueName { get; set; }
    }
}
