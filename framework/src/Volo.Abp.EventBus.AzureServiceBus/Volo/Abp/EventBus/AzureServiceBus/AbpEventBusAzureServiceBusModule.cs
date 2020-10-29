using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    public class AbpEventBusAzureServiceBusModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpAzureServiceBusEventBusOptions>(configuration.GetSection("RabbitMQ:EventBus"));
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
