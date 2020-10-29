using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;

namespace Volo.Abp.AzureServiceBus
{
    public interface IServiceBusConnectionPool : IDisposable
    {
        ServiceBusConnection Get(string connectionName = null);
    }
}
