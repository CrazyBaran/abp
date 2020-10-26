using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Volo.Abp.AzureServiceBus
{
    public interface ITopicClientPool : IDisposable
    {
        ITopicClient Get(string topic, string connectionName = null);
    }
}
