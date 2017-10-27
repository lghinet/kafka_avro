using System;
using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Core
{
    public class TopicCommandResolver : ITopicCommandResolver
    {
        private Dictionary<string, Type> _mappings;

        public TopicCommandResolver()
        {
            _mappings = new Dictionary<string, Type>();
        }

        public void Register(string key, Type type)
        {
            _mappings.Add(key, type);
        }

        public Type GetCommandType(string key)
        {
            if (_mappings.ContainsKey(key))
                return _mappings[key];

            throw new KeyNotFoundException($"topic {key} does not have a registered command");
        }
    }
}
