using Confluent.Kafka.Serialization;
using Messaging.Transport.Kafka.Serialization;

namespace Messaging.Transport.Kafka.Avro.Serialization
{
    public class AvroSerializer<T> : ISerializer<T>
    {
        private readonly IKafkaSerializer _kafkaSerializer;
        private readonly string _topic;
        private readonly bool _isKey;

        public AvroSerializer(IKafkaSerializer kafkaSerializer, string topic, bool isKey)
        {
            _kafkaSerializer = kafkaSerializer;
            _topic = topic;
            _isKey = isKey;
        }
        public byte[] Serialize(T data)
        {
            return _kafkaSerializer.SerializeAsync(data, _isKey, _topic).Result;
        }
    }
}
