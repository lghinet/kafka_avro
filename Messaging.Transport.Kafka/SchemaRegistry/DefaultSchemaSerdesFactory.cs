using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Messaging.Transport.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Transport.Kafka.SchemaRegistry
{
    public class DefaultSchemaSerdesFactory : ISchemaSerializerFactory, ISchemaDeserializerFactory
    {
        private readonly Dictionary<Type, object> _serList;
        private readonly Dictionary<Type, object> _desList;

        public DefaultSchemaSerdesFactory()
        {
            _serList = new Dictionary<Type, object>()
            {
                {typeof(String), new StringSerializer(Encoding.UTF8)},
                {typeof(Int32), new IntSerializer()},
                {typeof(Null), new NullSerializer()}
            };

            _desList = new Dictionary<Type, object>()
            {
                {typeof(String), new StringDeserializer(Encoding.UTF8)},
                {typeof(Int32), new IntDeserializer()},
                {typeof(Null), new NullDeserializer()}
            };
        }

        public ISerializer<T> GetSerializer<T>(bool isKey, string topic)
        {
            if (_serList.ContainsKey(typeof(T)))
                return (ISerializer<T>)_serList[typeof(T)];

            //default
            return new JsonSerializer<T>(Encoding.UTF8);
            //throw new KeyNotFoundException($"Serializer for type {typeof(T)} not registered");
        }

        public IDeserializer<T> GetDeserializer<T>()
        {
            if (_desList.ContainsKey(typeof(T)))
                return (IDeserializer<T>)_desList[typeof(T)];

            //default
            return new JsonDeserializer<T>(Encoding.UTF8);
        }

        public string GetSchema()
        {
            return "Default";
        }
    }
}