using System;
using System.Collections.Generic;
using System.Text;
using SimpleInjector.Integration.Messaging.Transport.Builder;

namespace SimpleInjector.Integration.Messaging.Transport.Kafka
{
    public class KafkaMessagingBuilder : IKafkaMessagingBuilder
    {
        public Container Container { get; }
        public bool HasConsumer { get; }
        public bool HasProducer { get; }

        public KafkaMessagingBuilder(IMessagingBuilder builder)
        {
            Container = builder.Container;
            HasConsumer = builder.HasConsumer;
            HasProducer = builder.HasProducer;
        }
    }
}
