using System;
using System.Linq;
using Microsoft.Hadoop.Avro;

namespace Messaging.Transport.Kafka.Avro.Surrogates
{
    class AvroSurrogateStrategy : IAvroSurrogate
    {
        private static readonly IAvroSurrogateStrategy[] Strategies = new IAvroSurrogateStrategy[]{new DateTimeSurrogate(), new GuidSurrogate()};
        public object GetDeserializedObject(object obj, Type targetType)
        {
            var surrogate = GetStrategy(targetType);
            if(surrogate != null)
            {
                return surrogate.GetDeserializedObject(obj, targetType);
            }

            return obj;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            var surrogate = GetStrategy(obj.GetType());
            if(surrogate != null)
            {
                return surrogate.GetObjectToSerialize(obj, targetType);
            }
            
            return obj;
        }

        public Type GetSurrogateType(Type type)
        {
            var surrogate = GetStrategy(type);
            if(surrogate != null)
            {
                return surrogate.GetSurrogateType(type);
            }
            
            return type;
        }

        private IAvroSurrogate GetStrategy(Type targetType)
        {
            return Strategies.FirstOrDefault(s => s.SurrogateFor(targetType));
        }
    }
}