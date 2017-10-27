using System;
using System.Collections;
using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Producer
{
    public interface IProducerConfiguration
    {
        IEnumerable<KeyValuePair<string, object>> ProducerConfiguration { get; set; }
    }
}