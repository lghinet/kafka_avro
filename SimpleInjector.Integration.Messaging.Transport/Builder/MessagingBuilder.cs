namespace SimpleInjector.Integration.Messaging.Transport.Builder
{
    public class MessagingBuilder : IMessagingBuilder
    {
        public Container Container { get; }
        public bool HasConsumer { get; internal set; }
        public bool HasProducer { get; internal set; }

        public MessagingBuilder(Container container)
        {
            Container = container;
        }
    }
}