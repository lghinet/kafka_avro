using Judo.SchemaRegistryClient;
using Messaging.Transport.Kafka.Avro.Serialization;
using Messaging.Transport.Kafka.SchemaRegistry;
using SimpleInjector.Advanced;
using System;

namespace SimpleInjector.Integration.Messaging.Transport.Kafka.Avro
{
    public static class AppExtensions
    {
        public static IKafkaMessagingBuilder WithAvroSchema(this IKafkaMessagingBuilder builder, string schemaRegistry)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (schemaRegistry == null)
                throw new ArgumentNullException(nameof(schemaRegistry));

            builder.Container.RegisterSingleton(() => new CachedSchemaRegistryClient(schemaRegistry, 200));
            builder.Container.AppendToCollection(typeof(ISchemaSerializerFactory), typeof(SchemaSerializerFactory));
            builder.Container.AppendToCollection(typeof(ISchemaDeserializerFactory), typeof(SchemaDeserializerFactory));

            return builder;
        }
    }
}
