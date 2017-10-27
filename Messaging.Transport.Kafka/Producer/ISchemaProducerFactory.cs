using System;
using Confluent.Kafka;

namespace Messaging.Transport.Kafka.Producer
{
    public interface ISchemaProducerFactory : IDisposable
    {
        ISerializingProducer<TKey, TValue> GetSerializingProducer<TKey, TValue>(string schema, string topic);
    }
}