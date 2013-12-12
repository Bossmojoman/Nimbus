﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Nimbus.InfrastructureContracts;
using Nimbus.MessageContracts;
using Castle.MicroKernel;

namespace Nimbus.Windsor.Infrastructure
{
    public class WindsorMulticastEventBroker : IMulticastEventBroker
    {
        private readonly IKernel _container;

        public WindsorMulticastEventBroker(IKernel container)
        {
            _container = container;
        }

        public void PublishMulticast<TBusEvent>(TBusEvent busEvent) where TBusEvent : IBusEvent
        {
            using (var scope = _container.BeginScope())
            {
                var type = typeof (IEnumerable<IHandleMulticastEvent<TBusEvent>>);
                var handlers = (IEnumerable) _container.Resolve(type);
                foreach (var handler in handlers.Cast<IHandleMulticastEvent<TBusEvent>>()) handler.Handle(busEvent);
            }
        }
    }
}