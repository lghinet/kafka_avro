using Messaging.Transport.Abstractions.Consumer;
using Messaging.Transport.Abstractions.Producer;
using Messaging.Transport.Kafka.Consumer;
using Messaging.Transport.Kafka.Producer;
using Messaging.Transport.Kafka.SchemaRegistry;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Messaging.Transport.Builder;
using System;

namespace SimpleInjector.Integration.Messaging.Transport.Kafka
{
    public static class AppExtensions
    {
        public static IKafkaMessagingBuilder WithKafkaTransport(this IMessagingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (builder.Container == null)
                throw new ArgumentNullException(nameof(builder.Container));

            if (builder.HasProducer)
            {
                builder.Container.RegisterSingleton<SchemaSerializerProvider>();
                builder.Container.AppendToCollection(typeof(ISchemaSerializerFactory), typeof(DefaultSchemaSerdesFactory));
                builder.Container.Register(typeof(ISchemaProducerFactory), typeof(SchemaProducerFactory), Lifestyle.Singleton);
                builder.Container.AppendToCollection(typeof(IProducer), typeof(KafkaProducer));
            }

            if (builder.HasConsumer)
            {
                builder.Container.RegisterSingleton<IConsumerService, KafkaConsumerService>();
                builder.Container.RegisterSingleton<SchemaDeserializerProvider>();
                builder.Container.AppendToCollection(typeof(ISchemaDeserializerFactory), typeof(DefaultSchemaSerdesFactory));
                builder.Container.Register(typeof(ISchemaConsumerFactory), typeof(SchemaConsumerFactory), Lifestyle.Singleton);
            }
            return new KafkaMessagingBuilder(builder);
        }
    }
}