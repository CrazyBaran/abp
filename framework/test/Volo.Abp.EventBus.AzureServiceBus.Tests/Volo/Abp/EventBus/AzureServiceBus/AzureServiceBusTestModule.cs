using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AzureServiceBus;
using Volo.Abp.Json;
using Volo.Abp.Json.Newtonsoft;
using Volo.Abp.Modularity;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    [DependsOn(typeof(AbpEventBusAzureServiceBusModule))]
    public class AzureServiceBusTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            base.ConfigureServices(context);
            context.Services.AddSingleton<IAzureServiceBusSerializer, Utf8JsonAzureServiceBusSerializer>();
            context.Services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
            context.Services.AddSingleton<ITopicClientPool, TopicClientPool>();
            context.Services.AddSingleton<ISubscriptionClientPool, SubscriptionClientPool>();
            context.Services.AddSingleton<IServiceBusConnectionPool, ServiceBusConnectionPool>();
            context.Services.AddSingleton<TopicInitializer>();
            context.Services.AddSingleton<IManagementClientPool, ManagementClientPool>();
        }
    }
}
