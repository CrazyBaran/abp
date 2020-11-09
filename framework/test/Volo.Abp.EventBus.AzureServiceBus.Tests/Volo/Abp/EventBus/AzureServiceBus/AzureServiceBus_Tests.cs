using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    public class AzureServiceBus_Tests : AzureServiceBusTestBase
    {
        [Fact]
        public async Task Should_Create_Topic_On_Publish()
        {
            await DistributedEventBus.PublishAsync(new MySimpleEventData(1, Guid.NewGuid()));
        }
    }
}
