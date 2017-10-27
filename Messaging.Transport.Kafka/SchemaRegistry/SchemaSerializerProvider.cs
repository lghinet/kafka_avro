using Confluent.Kafka.Serialization;
using Messaging.Transport.Abstractions.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Messaging.Transport.Kafka.SchemaRegistry
{
    public class SchemaSerializerProvider
    {
        private readonly List<ISchemaSerializerFactory> _list;

        public SchemaSerializerProvider(MultiInstanceFactory multiInstanceFactory)
        {
            _list = multiInstanceFactory(typeof(ISchemaSerializerFactory)).Cast<ISchemaSerializerFactory>().ToList();
        }

        public ISerializer<TValue> GetSerializer<TValue>(string schema, bool isKey, string topic)
        {
            var ser = _list.FirstOrDefault(x => x.GetSchema() == schema);

            if (ser != null)
                return ser.GetSerializer<TValue>(isKey, topic);

            throw new KeyNotFoundException($"SerializerFactory for type {typeof(TValue)} not registered");
        }
    }
}