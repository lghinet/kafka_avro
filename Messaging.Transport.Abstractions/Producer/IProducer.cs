using System.Threading.Tasks;

namespace Messaging.Transport.Abstractions.Producer
{
    public interface IProducer
    {
        Task<IMessageResponse> ProduceAsync<TPayload>(IMessage<TPayload> message);
        Task<IMessageResponse> ProduceAsync<TKey, TPayload>(TKey key, IMessage<TPayload> message);
        Task<IBatchMessageResponse> ProduceAsync<TPayload, TChunk>(IBatchMessage<TPayload, TChunk> message) where TPayload : class, IBatchData<TChunk>;
        Task<IBatchMessageResponse> ProduceAsync<TKey, TPayload, TChunk>(TKey key, IBatchMessage<TPayload, TChunk> message) where TPayload : class, IBatchData<TChunk>;
    }
}