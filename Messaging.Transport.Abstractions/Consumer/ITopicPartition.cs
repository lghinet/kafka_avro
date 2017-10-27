using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Transport.Abstractions.Consumer
{
    public interface ITopicPartition
    {
        string Topic { get; }
        int Partition { get; }
    }
}
