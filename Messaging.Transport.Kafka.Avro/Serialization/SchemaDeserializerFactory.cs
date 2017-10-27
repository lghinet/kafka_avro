using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Judo.SchemaRegistryClient;
using Messaging.Transport.Kafka.SchemaRegistry;

namespace Messaging.Transport.Kafka.Avro.Serialization
{
    public class SchemaDeserializerFactory : ISchemaDeserializerFactory
    {
        private readonly CachedSchemaRegistryClient _cachedSchemaRegistryClient;

        public SchemaDeserializerFactory(CachedSchemaRegistryClient cachedSchemaRegistryClient)
        {
            _cachedSchemaRegistryClient = cachedSchemaRegistryClient;
        }

        public IDeserializer<T> GetDeserializer<T>()
        {
            if (typeof(T) == typeof(Null))
                return null;

            var judaSerializer = new SchemaRegistryAvroSerializer(_cachedSchemaRegistryClient, true);
            return new AvroDeserializer<T>(judaSerializer);
        }

        public string GetSchema()
        {
            return "Avro";
        }
    }
}