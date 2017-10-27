namespace Messaging.Transport.Abstractions.Producer
{
    public interface IMessageConfigurationProvider<out TMessageConfiguration>
        where TMessageConfiguration : IMessageConfiguration, new()
    {
        TMessageConfiguration GetMessageConfiguration(string messageType);
    }
}