using Messaging.Transport.Abstractions.Consumer;
using Messaging.Transport.Kafka.SchemaRegistry;

namespace Messaging.Transport.Kafka.Consumer
{
    public class SchemaConsumerFactory : ISchemaConsumerFactory
    {
        private readonly SchemaDeserializerProvider _schemaDeserializerProvider;
        private readonly IConsumerConfiguration _consumerConfiguration;

        public SchemaConsumerFactory(SchemaDeserializerProvider schemaDeserializerProvider, IConsumerConfiguration consumerConfiguration)
        {
            _schemaDeserializerProvider = schemaDeserializerProvider;
            _consumerConfiguration = consumerConfiguration;
        }

        public KafkaConsumer<TKey, TValue> GetSerializingConsumer<TKey, TValue>(string schema)
        {
            var keyDeserializer = _schemaDeserializerProvider.GetDeserializer<TKey>(schema);
            var valueDeserializer = _schemaDeserializerProvider.GetDeserializer<TValue>(schema);
            return new KafkaConsumer<TKey, TValue>(_consumerConfiguration.ConsumerConfiguration, keyDeserializer, valueDeserializer);
        }

        public void Dispose()
        {
        }
    }
}