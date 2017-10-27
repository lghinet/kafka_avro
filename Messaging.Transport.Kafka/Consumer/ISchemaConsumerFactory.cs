using System;
using Messaging.Transport.Abstractions.Consumer;

namespace Messaging.Transport.Kafka.Consumer
{
    public interface ISchemaConsumerFactory:IDisposable
    {
        KafkaConsumer<TKey, TValue> GetSerializingConsumer<TKey, TValue>(string schema);
    }
}