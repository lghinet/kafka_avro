using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Judo.SchemaRegistryClient;
using Messaging.Transport.Kafka.SchemaRegistry;

namespace Messaging.Transport.Kafka.Avro.Serialization
{
    public class SchemaSerializerFactory : ISchemaSerializerFactory
    {
        private readonly CachedSchemaRegistryClient _cachedSchemaRegistryClient;

        public SchemaSerializerFactory(CachedSchemaRegistryClient cachedSchemaRegistryClient)
        {
            _cachedSchemaRegistryClient = cachedSchemaRegistryClient;
        }

        public ISerializer<T> GetSerializer<T>(bool isKey, string topic)
        {
            if (typeof(T) == typeof(Null))
                return null;

            var judaSerializer = new SchemaRegistryAvroSerializer(_cachedSchemaRegistryClient, true);
            return new AvroSerializer<T>(judaSerializer, topic, isKey);
        }

        public string GetSchema()
        {
            return "Avro";
        }
    }
}