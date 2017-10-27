using System;
using Microsoft.Hadoop.Avro;

namespace Messaging.Transport.Kafka.Avro.Surrogates
{
    interface IAvroSurrogateStrategy : IAvroSurrogate
    {
        bool SurrogateFor(Type type);
    }
}