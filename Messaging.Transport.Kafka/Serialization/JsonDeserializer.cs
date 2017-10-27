using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace Messaging.Transport.Kafka.Serialization
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly Encoding _encoding;

        public JsonDeserializer(Encoding encoding)
        {
            this._encoding = encoding;
        }

        public T Deserialize(byte[] data)
        {
            if (data == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(this._encoding.GetString(data));
        }
    }
}