using Messaging.Transport.Abstractions.Producer;
using System.Collections.Generic;

namespace SimpleInjector.Integration.Messaging.Transport.Config
{
    public class ProducerConfig : IProducerConfiguration
    {
        public IEnumerable<KeyValuePair<string, object>> ProducerConfiguration { get; set; }
        public List<Dictionary<string, string>> MessageConfigurations { get; set; }
    }
}
