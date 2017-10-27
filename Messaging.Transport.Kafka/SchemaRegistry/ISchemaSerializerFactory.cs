using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka.Serialization;

namespace Messaging.Transport.Kafka.SchemaRegistry
{
    public interface ISchemaSerializerFactory
    {
        ISerializer<T> GetSerializer<T>(bool isKey, string topic);
        string GetSchema();
    }
}
