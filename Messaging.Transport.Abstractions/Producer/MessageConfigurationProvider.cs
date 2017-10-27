using System.Collections.Generic;
using System.Linq;

namespace Messaging.Transport.Abstractions.Producer
{
    public class MessageConfigurationProvider<TMessageConfiguration> : IMessageConfigurationProvider<TMessageConfiguration>
        where TMessageConfiguration : IMessageConfiguration, new()
    {
        private readonly List<TMessageConfiguration> _configs;

        public MessageConfigurationProvider(IEnumerable<MessageConfiguration> configurations)
        {
            _configs = configurations.Where(x => x.TransportType == TransportType || string.IsNullOrEmpty(TransportType)).Select(x =>
            {
                var r = new TMessageConfiguration();
                r.Init(x.Settings);
                return r;
            }).ToList();
        }

        public virtual string TransportType => typeof(TMessageConfiguration).Name
            .Substring(0, typeof(TMessageConfiguration).Name.Length - "MessageConfiguration".Length);

        public TMessageConfiguration GetMessageConfiguration(string messageType)
        {
            var cfg = _configs.FirstOrDefault(x => x.MessageType == messageType);
            if (cfg == null)
                throw new KeyNotFoundException($"MessageConfiguration for {messageType} not found");
            return cfg;
        }
    }
}