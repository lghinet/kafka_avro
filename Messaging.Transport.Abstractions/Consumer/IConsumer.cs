using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Messaging.Transport.Abstractions.Producer;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IConsumer : IDisposable
    {
        void Poll(TimeSpan millisecondsTimeout);
        List<string> Subscription { get; }
        void Subscribe(IEnumerable<string> topics);
        void Subscribe(string topic);
        void Unsubscribe();

        //event Action<List<ITopicPartition>> OnPartitionsAssigned;

        //event Action<List<ITopicPartition>> OnPartitionsRevoked;

        //event Action<string> OnError;
        //void Assign(List<ITopicPartition> partitions);
    }

    public interface IConsumer<out TKey, out TValue> : IConsumer
    {
        //event Action<IMessage<TKey, TValue>> OnMessage;
        //event Action<IMessage<TKey, TValue>> OnConsumeError;
    }
}