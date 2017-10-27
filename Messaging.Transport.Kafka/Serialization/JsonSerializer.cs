using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace Messaging.Transport.Kafka.Serialization
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        private Encoding encoding;

        /// <summary>
        ///     Initializes a new StringSerializer class instance.
        /// </summary>
        /// <param name="encoding">The encoding to use when serializing.</param>
        public JsonSerializer(Encoding encoding)
        {
            this.encoding = encoding;
        }

        /// <summary>Encodes a string value in a byte array.</summary>
        /// <param name="val">The string value to serialize.</param>
        /// <returns>
        ///     <paramref name="val" /> encoded in a byte array (or null if <paramref name="val" /> is null).
        /// </returns>
        public byte[] Serialize(T val)
        {
            if (val == null)
                return (byte[]) null;
            return this.encoding.GetBytes(JsonConvert.SerializeObject(val));
        }
    }
}