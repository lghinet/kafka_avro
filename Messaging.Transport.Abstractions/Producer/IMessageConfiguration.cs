using System.Collections;
using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Producer
{
    public interface IMessageConfiguration
    {
        void Init(IDictionary<string, string> settings);
        string MessageType { set; get; }
        int BatchSize { set; get; }
    }
}
