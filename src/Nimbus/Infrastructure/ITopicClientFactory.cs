﻿using System;
using Microsoft.ServiceBus.Messaging;

namespace Nimbus.Infrastructure
{
    [Obsolete("We should be using a dependency on some kind of INimbusEventSender")]
    internal interface ITopicClientFactory
    {
        TopicClient GetTopicClient(Type busEventType);
    }
}