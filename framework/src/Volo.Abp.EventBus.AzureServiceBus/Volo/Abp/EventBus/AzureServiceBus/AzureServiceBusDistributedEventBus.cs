using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public ISubscriptionClientPool SubscriptionClientPool { get; }
        protected ConcurrentDictionary<Type, List<IEventHandlerFactory>> HandlerFactories { get; }
        protected ConcurrentDictionary<string, Type> EventTypes { get; }

        public AzureServiceBusDistributedEventBus(
            IOptions<AbpAzureServiceBusEventBusOptions> options,
            IAzureServiceBusSerializer serializer,
            ITopicClientPool topicClientPool,
            ISubscriptionClientPool subscriptionClientPool,
            IServiceScopeFactory serviceScopeFactory,
            ICurrentTenant currentTenant)
            : base(serviceScopeFactory, currentTenant)
        {
            AbpAzureServiceBusEventBusOptions = options.Value;
            Serializer = serializer;
            TopicClientPool = topicClientPool;
            SubscriptionClientPool = subscriptionClientPool;
        }

        public void Initialize()
        {
           // Consumer = MessageConsumerFactory.Create(
           //     new ExchangeDeclareConfiguration(
           //         AbpRabbitMqEventBusOptions.ExchangeName,
           //         type: "direct",
           //         durable: true
           //     ),
           //     new QueueDeclareConfiguration(
           //         AbpRabbitMqEventBusOptions.ClientName,
           //         durable: true,
           //         exclusive: false,
           //         autoDelete: false
           //     ),
           //     AbpRabbitMqEventBusOptions.ConnectionName
           // );
           //
           // Consumer.OnMessageReceived(ProcessEventAsync);
           //
           // SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
        }

        public override async Task PublishAsync(Type eventType, object eventData)
        {
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            var body = Serializer.Serialize(eventData);

            var topicClient = await TopicClientPool.Get(eventName, AbpAzureServiceBusEventBusOptions.ConnectionName);

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
            var handlerFactories = GetOrCreateHandlerFactories(eventType);

            if (factory.IsInFactories(handlerFactories))
            {
                return NullDisposable.Instance;
            }

            handlerFactories.Add(factory);

            if (handlerFactories.Count == 1) //TODO: Multi-threading!
            {
                var eventName = EventNameAttribute.GetNameOrDefault(eventType);
                var subscriptionName = factory.GetHandler().EventHandler.GetType().Name;
            }

            return new EventHandlerFactoryUnregistrar(this, eventType, factory);
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return HandlerFactories.GetOrAdd(
                eventType,
                type =>
                {
                    var eventName = EventNameAttribute.GetNameOrDefault(type);
                    EventTypes[eventName] = type;
                    return new List<IEventHandlerFactory>();
                }
            );
        }

        public override void Unsubscribe<TEvent>(Func<TEvent, Task> action)
        {
            Check.NotNull(action, nameof(action));

            GetOrCreateHandlerFactories(typeof(TEvent))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEvent>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }

        public override void Unsubscribe(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                    );
    });
        }

        public override void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        public override void UnsubscribeAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }

        protected override IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in HandlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type targetEventType, Type handlerEventType)
        {
            //Should trigger same type
            if (handlerEventType == targetEventType)
            {
                return true;
            }

            //TODO: Support inheritance? But it does not support on subscription to RabbitMq!
            //Should trigger for inherited types
            if (handlerEventType.IsAssignableFrom(targetEventType))
            {
                return true;
            }

            return false;
        }
    }
}
