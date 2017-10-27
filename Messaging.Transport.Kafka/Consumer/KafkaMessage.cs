using Confluent.Kafka;
using Messaging.Transport.Abstractions.Consumer;

namespace Messaging.Transport.Kafka.Consumer
{
    public class KafkaMessage<TKey, TValue> : IMessage<TKey, TValue>
    {
        /// <summary>Gets the topic name associated with this message.</summary>
        public string Topic { get; }

        /// <summary>Gets the partition associated with this message.</summary>
        public int Partition { get; }

        /// <summary>
        ///     Gets the offset of this message in the Kafka topic partition.
        /// </summary>
        public Offset Offset { get; }

        /// <summary>Gets the message key value.</summary>
        public TKey Key { get; }

        /// <summary>Gets the message value.</summary>
        public TValue Value { get; }

        /// <summary>Gets the message timestamp.</summary>
        public Timestamp Timestamp { get; }

        /// <summary>
        ///     Gets a rich <see cref="P:Confluent.Kafka.Message`2.Error" /> associated with the message.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        ///     Gets the topic/partition/offset associated with this message.
        /// </summary>
        public TopicPartitionOffset TopicPartitionOffset => new TopicPartitionOffset(this.Topic, this.Partition, this.Offset);

        /// <summary>
        ///     Gets the topic/partition associated with this message.
        /// </summary>
        public TopicPartition TopicPartition => new TopicPartition(this.Partition, this.Topic);

        /// <summary>Instantiates a new Message class instance.</summary>
        /// <param name="topic">
        ///     The Kafka topic name associated with this message.
        /// </param>
        /// <param name="partition">
        ///     The topic partition id associated with this message.
        /// </param>
        /// <param name="offset">
        ///     The offset of this message in the Kafka topic partition.
        /// </param>
        /// <param name="key">The message key value.</param>
        /// <param name="val">The message value.</param>
        /// <param name="timestamp">The message timestamp.</param>
        /// <param name="error">
        ///     A rich <see cref="P:Confluent.Kafka.Message`2.Error" /> associated with the message.
        /// </param>
        public KafkaMessage(string topic, int partition, long offset, TKey key, TValue val, Timestamp timestamp, Error error)
        {
            this.Topic = topic;
            this.Partition = partition;
            this.Offset = (Offset) offset;
            this.Key = key;
            this.Value = val;
            this.Timestamp = timestamp;
            this.Error = error;
        }

        public KafkaMessage(Message<TKey, TValue> msg)
            : this(msg.Topic, msg.Partition, msg.Offset, msg.Key, msg.Value, msg.Timestamp, msg.Error)
        {
        }
    }
}