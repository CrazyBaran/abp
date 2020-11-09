using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AzureServiceBus
{
    [Serializable]
    public class AzureServiceBusConnection
    {
        public string ConnectionString { get; set; }
        public bool IsAutomaticlyCreateTopicAndSubscriptions { get; set; }
        public string SasKeyName { get; set; }
        public string SasKeyValue { get; set; }
    }
}
