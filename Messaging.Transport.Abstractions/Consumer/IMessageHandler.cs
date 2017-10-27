using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IMessageHandler<in TKey, in TValue>
    {
        Task HandleIncomingMessage(IMessage<TKey, TValue> message, CancellationToken token = default(CancellationToken));
    }

    public interface IMessageHandler
    {
        Task HandleIncomingMessage<TKey, TValue>(IMessage<TKey, TValue> message, CancellationToken token = default(CancellationToken));
    }
}