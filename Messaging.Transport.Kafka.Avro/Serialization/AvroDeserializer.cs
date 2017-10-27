using Confluent.Kafka.Serialization;
using Messaging.Transport.Kafka.Serialization;

namespace Messaging.Transport.Kafka.Avro.Serialization
{
    public class AvroDeserializer<T> : IDeserializer<T>
    {
        private readonly IKafkaSerializer _kafkaSerializer;

        public AvroDeserializer(IKafkaSerializer kafkaSerializer)
        {
            _kafkaSerializer = kafkaSerializer;
        }

        public T Deserialize(byte[] data)
        {
            if (data == null)
                return default(T);

            return _kafkaSerializer.DeserializeAsync<T>(data).Result;
        }
    }
}
