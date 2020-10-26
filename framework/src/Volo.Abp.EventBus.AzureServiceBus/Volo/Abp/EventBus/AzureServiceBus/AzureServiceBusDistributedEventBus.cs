using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AzureServiceBus;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;

using Volo.Abp.Threading;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IDistributedEventBus), typeof(AzureServiceBusDistributedEventBus))]
    public class AzureServiceBusDistributedEventBus : EventBusBase, IDistributedEventBus, ISingletonDependency
    {
        protected AbpAzureServiceBusEventBusOptions AbpAzureServiceBusEventBusOptions { get; }
        protected IAzureServiceBusSerializer Serializer { get; }
        protected ITopicClientPool TopicClientPool { get; }

        public AzureServiceBusDistributedEventBus(
            IOptions<AbpAzureServiceBusEventBusOptions> options,
            IAzureServiceBusSerializer serializer,
            ITopicClientPool topicClientPool,
            IServiceScopeFactory serviceScopeFactory,
            ICurrentTenant currentTenant)
            : base(serviceScopeFactory, currentTenant)
        {
            AbpAzureServiceBusEventBusOptions = options.Value;
            Serializer = serializer;
            TopicClientPool = topicClientPool;
        }

        public override async Task PublishAsync(Type eventType, object eventData)
        {
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            var body = Serializer.Serialize(eventData);

            var topicClient = TopicClientPool.Get(AbpAzureServiceBusEventBusOptions.ConnectionName, eventName);

            await topicClient.SendAsync(new Message()
            {
                Body = body
            });
        }

        public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }

        public override IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            throw new NotImplementedException();
        }

        public override void UnsubscribeAll(Type eventType)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            throw new NotImplementedException();
        }
    }
}
