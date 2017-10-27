using Messaging.Transport.Abstractions.Consumer;

namespace Messaging.Transport.Kafka.Consumer
{
    public class TopicPartition : ITopicPartition
    {
        public TopicPartition(int partition, string topic)
        {
            Partition = partition;
            Topic = topic;
        }

        public string Topic { get; }
        public int Partition { get; }

        public override string ToString()
        {
            return $"{Topic} [{Partition}]";
        }
    }
}
