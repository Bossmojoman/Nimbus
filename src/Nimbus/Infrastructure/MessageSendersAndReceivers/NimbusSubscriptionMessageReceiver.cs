﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Nimbus.Extensions;

namespace Nimbus.Infrastructure.MessageSendersAndReceivers
{
    internal class NimbusSubscriptionMessageReceiver : INimbusMessageReceiver
    {
        private readonly IQueueManager _queueManager;
        private readonly string _topicPath;
        private readonly string _subscriptionName;

        private readonly Lazy<SubscriptionClient> _subscriptionClient;

        public NimbusSubscriptionMessageReceiver(IQueueManager queueManager, string topicPath, string subscriptionName)
        {
            _queueManager = queueManager;
            _topicPath = topicPath;
            _subscriptionName = subscriptionName;

            _subscriptionClient = new Lazy<SubscriptionClient>(CreateMessageReceiver, LazyThreadSafetyMode.PublicationOnly);
        }

        public Task WaitUntilReady()
        {
            return Task.Run(() => { var dummy = _subscriptionClient.Value; });
        }

        public Task<IEnumerable<BrokeredMessage>> Receive(int batchSize)
        {
            return _subscriptionClient.Value.ReceiveBatchAsync(batchSize, TimeSpan.FromSeconds(1));
        }

        private SubscriptionClient CreateMessageReceiver()
        {
            return _queueManager.CreateSubscriptionReceiver(_topicPath, _subscriptionName);
        }

        public override string ToString()
        {
            return "{0}/{1}".FormatWith(_topicPath, _subscriptionName);
        }

        public void Dispose()
        {
            if (!_subscriptionClient.IsValueCreated) return;
            _subscriptionClient.Value.Close();
        }
    }
}