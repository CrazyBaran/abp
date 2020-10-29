using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Volo.Abp.AzureServiceBus
{
    public interface ISubscriptionClientPool : IDisposable
    {
        ISubscriptionClient Get(string topic, string subscriptionName, string connectionName = null);
    }
}
