using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceBus;

namespace Volo.Abp.AzureServiceBus
{
    public interface INamespaceManagerPool
    {
        NamespaceManager Get(string connectionName = null);
    }
}
