using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Volo.Abp.AzureServiceBus
{
    public interface ITopicClientPool : IDisposable
    {
        Task<ITopicClient> Get(string topic, string connectionName = null);
    }
}
