using System;
using SimpleInjector.Integration.Messaging.Transport.Config;

namespace SimpleInjector.Integration.Messaging.Transport.Builder
{
    public class MessagingOptions
    {
        public bool HasConsumer { get; internal set; }
        public bool HasProducer { get; internal set; }

        public MessagingOptions(Container container)
        {
            Container = container;
        }

        public Container Container { get; }

    }
}