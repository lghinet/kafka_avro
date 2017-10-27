using System;
using System.Collections.Concurrent;
using Confluent.Kafka;
using Messaging.Transport.Abstractions.Producer;
using Messaging.Transport.Kafka.SchemaRegistry;

namespace Messaging.Transport.Kafka.Producer
{
    public class SchemaProducerFactory : ISchemaProducerFactory
    {
        private readonly SchemaSerializerProvider _schemaSerializerProvider;
        private readonly IProducerConfiguration _producerConfiguration;
        private readonly ConcurrentDictionary<string, IDisposable> _list;

        public SchemaProducerFactory(SchemaSerializerProvider schemaSerializerProvider, IProducerConfiguration producerConfiguration)
        {
            _schemaSerializerProvider = schemaSerializerProvider;
            _producerConfiguration = producerConfiguration;
            _list = new ConcurrentDictionary<string, IDisposable>();
        }

        public ISerializingProducer<TKey, TValue> GetSerializingProducer<TKey, TValue>(string schema, string topic)
        {
            var key = $"{schema}_{topic}";
            return (ISerializingProducer<TKey, TValue>) _list.GetOrAdd(key, _ => CreateProducer<TKey, TValue>(schema, topic));
        }

        private IDisposable CreateProducer<TKey, TValue>(string schema, string topic)
        {
            var keySerializer = _schemaSerializerProvider.GetSerializer<TKey>(schema, true, topic);
            var valueSerializer = _schemaSerializerProvider.GetSerializer<TValue>(schema, false, topic);
            return new Producer<TKey, TValue>(_producerConfiguration.ProducerConfiguration, keySerializer, valueSerializer);
        }

        public void Dispose()
        {
            foreach (var prod in _list.Values)
                prod.Dispose();
        }
    }
}