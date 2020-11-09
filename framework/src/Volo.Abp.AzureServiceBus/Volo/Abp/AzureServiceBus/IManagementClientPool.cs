using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus.Management;

namespace Volo.Abp.AzureServiceBus
{
    public interface IManagementClientPool
    {
        ManagementClient Get(string connectionName = null);
    }
}
