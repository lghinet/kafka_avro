using SimpleInjector.Integration.Messaging.Transport.Config;

namespace SimpleInjector.Integration.Messaging.Transport.Builder
{
    public interface IMessagingBuilder
    {
        Container Container { get; }
        bool HasConsumer { get; }
        bool HasProducer { get; }
    }

}