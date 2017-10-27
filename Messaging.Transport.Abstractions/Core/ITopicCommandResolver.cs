using System;

namespace Messaging.Transport.Abstractions.Core
{
    public interface ITopicCommandResolver
    {
        void Register(string key, Type type);
        Type GetCommandType(string key);
    }
}
