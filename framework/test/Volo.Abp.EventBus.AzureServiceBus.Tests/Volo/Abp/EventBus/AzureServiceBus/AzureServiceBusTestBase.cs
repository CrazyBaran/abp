
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Testing;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    public abstract class AzureServiceBusTestBase : AbpIntegratedTest<AzureServiceBusTestModule>
    {
        protected IDistributedEventBus DistributedEventBus;

        protected AzureServiceBusTestBase()
        {
            DistributedEventBus = GetRequiredService<AzureServiceBusDistributedEventBus>();
        }

        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
