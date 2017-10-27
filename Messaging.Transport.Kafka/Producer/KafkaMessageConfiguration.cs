using System;
using Messaging.Transport.Abstractions.Producer;
using System.Collections.Generic;

namespace Messaging.Transport.Kafka.Producer
{
    public class KafkaMessageConfiguration : IMessageConfiguration
    {
        public string MessageType { set; get; }
        public int BatchSize { get; set; }
        public string Topic { set; get; }
        public string Schema { set; get; }

        public void Init(IDictionary<string, string> settings)
        {
            Topic = settings["Topic"];
            MessageType = settings["MessageType"];
            Schema = settings.ContainsKey("Schema") ? settings["Schema"] : "Default";
            BatchSize = settings.ContainsKey("BatchSize") ? Convert.ToInt32(settings["BatchSize"]) : 1;
        }
    }
}