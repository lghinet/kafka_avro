using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IConsumerService : IDisposable
    {
        Task Start();
        void Stop();
        IConsumerService AddConsumer<TKey, TValue>(Func<IMessage<TKey, TValue>, CancellationToken, Task> onMessageReceived = null);
    }
}
