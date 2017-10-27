using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka.Serialization;

namespace Messaging.Transport.Kafka.SchemaRegistry
{
    public interface ISchemaDeserializerFactory
    {
        IDeserializer<T> GetDeserializer<T>();
        string GetSchema();
    }
}
