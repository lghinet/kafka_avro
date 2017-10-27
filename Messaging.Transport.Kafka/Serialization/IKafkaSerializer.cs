using System.Threading.Tasks;

namespace Messaging.Transport.Kafka.Serialization
{
    public interface IKafkaSerializer
    {
        Task<byte[]> SerializeAsync<TPayload>(TPayload payload, bool isKey, string topic);

        Task<TPayload> DeserializeAsync<TPayload>(byte[] payload);
    }
}