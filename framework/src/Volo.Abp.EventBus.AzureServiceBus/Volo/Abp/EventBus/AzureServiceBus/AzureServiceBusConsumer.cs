using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Volo.Abp.EventBus.AzureServiceBus.Volo.Abp.EventBus.AzureServiceBus;
using Volo.Abp.EventBus.Distributed;

namespace Volo.Abp.EventBus.AzureServiceBus
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private ISubscriptionClient Client;
        private IEventHandlerFactory Factory;

        public static AzureServiceBusConsumer CreateAzureServiceBusConsumer(string topicName, string subscriptionName,
            string connectionString, IEventHandlerFactory factory)
        {
            var subscriptionClient = new SubscriptionClient(connectionString, topicName, subscriptionName);
            var azureSericeBusConsumer = new AzureServiceBusConsumer(
                subscriptionClient,
                factory);

            azureSericeBusConsumer.RegisterOnMessageHandlerAndReceiveMessages();
            return azureSericeBusConsumer;
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {

        }

        protected AzureServiceBusConsumer(ISubscriptionClient subscriptionClient, 
            IEventHandlerFactory factory)
        {
            Client = subscriptionClient;
            Factory = factory;
        }
    }
}
