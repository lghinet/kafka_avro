using System.Collections.Generic;
using Messaging.Transport.Abstractions.Core;

namespace Messaging.Transport.Abstractions.Producer
{
    public class ProducerFactory : IProducerFactory
    {
        private readonly MultiInstanceFactory _factory;
        private readonly IMessageConfigurationProvider<MessageConfiguration> _messageConfigProvider;

        public ProducerFactory(MultiInstanceFactory factory, IMessageConfigurationProvider<MessageConfiguration> messageConfigProvider)
        {
            _factory = factory;
            _messageConfigProvider = messageConfigProvider;
        }

        public IProducer GetProducer(string messageType)
        {
            var transportType = _messageConfigProvider.GetMessageConfiguration(messageType).TransportType;
            var list = _factory(typeof(IProducer));
            return FindProducer(list, transportType) as IProducer;
        }

        private object FindProducer(IEnumerable<object> list, string transportType)
        {
            foreach (var producer in list)
                if (producer.GetType().Namespace.Split('.')[2] == transportType)
                    return producer;

            throw new KeyNotFoundException($"Producer for {transportType} not registered");
        }
    }
}