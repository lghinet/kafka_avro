using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IMessageContextHandler<in TKey, in TValue>
    {
        Task HandleIncomingMessageContext(IMessage<TKey, TValue> message, CancellationToken token = default(CancellationToken));
    }
}