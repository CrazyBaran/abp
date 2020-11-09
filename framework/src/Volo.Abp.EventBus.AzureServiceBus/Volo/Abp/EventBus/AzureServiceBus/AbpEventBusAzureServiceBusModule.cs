using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    [DependsOn(typeof(AbpMultiTenancyModule))]
    public class AbpEventBusAzureServiceBusModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpAzureServiceBusEventBusOptions>(configuration.GetSection("Azure:ServiceBus"));
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            context
                .ServiceProvider
                .GetRequiredService<AzureServiceBusDistributedEventBus>()
                .Initialize();
        }
    }
}
