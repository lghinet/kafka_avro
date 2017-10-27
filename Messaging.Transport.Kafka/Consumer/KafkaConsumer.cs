using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka.Serialization;
using Messaging.Transport.Abstractions.Consumer;

namespace Messaging.Transport.Kafka.Consumer
{
    public class KafkaConsumer<TKey, TValue> : Confluent.Kafka.Consumer<TKey, TValue>, IConsumer<TKey, TValue>
    {
        //public new event Action<List<ITopicPartition>> OnPartitionsAssigned;
        //public new event Action<List<ITopicPartition>> OnPartitionsRevoked;
        //public new event Action<IMessage<TKey, TValue>> OnMessage;

        public KafkaConsumer(IEnumerable<KeyValuePair<string, object>> config, IDeserializer<TKey> keyDeserializer, IDeserializer<TValue> valueDeserializer)
            : base(config, keyDeserializer, valueDeserializer)
        {
            //base.OnMessage += Consumer_OnMessage;
            //base.OnPartitionsAssigned += Consumer_OnPartitionsAssigned;
            //base.OnPartitionsRevoked += Consumer_OnPartitionsRevoked;
        }

        //private void Consumer_OnPartitionsRevoked(object sender, List<Confluent.Kafka.TopicPartition> e)
        //{
        //    OnPartitionsRevoked?.Invoke(new List<ITopicPartition>(e.Select(a => new TopicPartition(a.Partition, a.Topic))));
        //}

        //private void Consumer_OnPartitionsAssigned(object sender, List<Confluent.Kafka.TopicPartition> e)
        //{
        //    OnPartitionsAssigned?.Invoke(new List<ITopicPartition>(e.Select(a => new TopicPartition(a.Partition, a.Topic))));
        //}

        //private void Consumer_OnMessage(object sender, Confluent.Kafka.Message<TKey, TValue> message)
        //{
        //    OnMessage?.Invoke(new KafkaMessage<TKey, TValue>(message));
        //}

        //public void Assign(List<ITopicPartition> partitions)
        //{
        //    base.Assign(partitions.Select(a => new Confluent.Kafka.TopicPartition(a.Topic, a.Partition)));
        //}
    }
}