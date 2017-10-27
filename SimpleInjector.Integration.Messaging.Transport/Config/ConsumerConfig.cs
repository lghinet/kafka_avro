using System.Collections.Generic;
using Messaging.Transport.Abstractions.Consumer;

namespace SimpleInjector.Integration.Messaging.Transport.Config
{
    public class ConsumerConfig : IConsumerConfiguration
    {
        public IEnumerable<KeyValuePair<string, object>> ConsumerConfiguration { get; set; }
        public List<MessageTopic> Topics { set; get; }
    }
}
