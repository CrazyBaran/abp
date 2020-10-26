using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.AzureServiceBus
{
    public interface IAzureServiceBusSerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] value, Type type);
    }
}
