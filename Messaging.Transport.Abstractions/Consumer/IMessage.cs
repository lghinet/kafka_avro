namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IMessage<out TKey, out TValue>
    {
        string Topic { get; }

        /// <summary>Gets the partition associated with this message.</summary>
        int Partition { get; }

        /// <summary>Gets the message key value.</summary>
        TKey Key { get; }

        /// <summary>Gets the message value.</summary>
        TValue Value { get; }
    }
}
