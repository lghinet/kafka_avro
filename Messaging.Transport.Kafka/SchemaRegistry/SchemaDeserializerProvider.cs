using Confluent.Kafka.Serialization;
using Messaging.Transport.Abstractions.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Messaging.Transport.Kafka.SchemaRegistry
{
    public class SchemaDeserializerProvider
    {
        private readonly List<ISchemaDeserializerFactory> _list;

        public SchemaDeserializerProvider(MultiInstanceFactory multiInstanceFactory)
        {
            _list = multiInstanceFactory(typeof(ISchemaDeserializerFactory)).Cast<ISchemaDeserializerFactory>().ToList();
        }

        public IDeserializer<TValue> GetDeserializer<TValue>(string schema)
        {
            var ser = _list.FirstOrDefault(x => x.GetSchema() == schema);

            if (ser != null)
                return ser.GetDeserializer<TValue>();

            throw new KeyNotFoundException($"DeserializerFactory for type {typeof(TValue)} with schema {schema} not registered");
        }
    }
}