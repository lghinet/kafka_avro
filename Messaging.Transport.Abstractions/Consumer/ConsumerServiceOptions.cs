using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Consumer
{
    public class ConsumerServiceOptions
    {
        public List<MessageTopic> Topics { set; get; }
    }

    public class MessageTopic
    {
        public string Topic { set; get; }
        public string Command { set; get; }
        public string TransportType { set; get; }
        public string Schema { set; get; }
    }
}
