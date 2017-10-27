using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface IConsumerConfiguration
    {
        IEnumerable<KeyValuePair<string, object>> ConsumerConfiguration { get; set; }
    }
}