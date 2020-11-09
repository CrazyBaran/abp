using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    public class MySimpleEventData : IMultiTenant
    {
        public int Value { get; set; }

        public Guid? TenantId { get; }

        public MySimpleEventData(int value, Guid? tenantId = null)
        {
            Value = value;
            TenantId = tenantId;
        }
    }
}
