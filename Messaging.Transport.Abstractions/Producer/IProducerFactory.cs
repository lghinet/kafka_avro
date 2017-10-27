namespace Messaging.Transport.Abstractions.Producer
{
    public interface IProducerFactory
    {
        IProducer GetProducer(string messageType);
    }
}