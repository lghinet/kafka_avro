using System;
using System.Threading;
using System.Threading.Tasks;
using Messaging.Transport.Abstractions.Consumer;
using SimpleInjector.Lifestyles;

namespace SimpleInjector.Integration.Messaging.Transport
{
    public class GenericMessageContext<TKey, TValue> : IMessageContextHandler<TKey, TValue>
    {
        private readonly Container _container;

        public GenericMessageContext(Container container)
        {
            _container = container;
        }

        public async Task HandleIncomingMessageContext(IMessage<TKey, TValue> msg, CancellationToken token = new CancellationToken())
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var messageHandlerRegistration = _container.GetRegistration(typeof(IMessageHandler<TKey, TValue>));

                if (messageHandlerRegistration != null)
                {
                    var messageHandler = messageHandlerRegistration.GetInstance() as IMessageHandler<TKey, TValue>;
                    await messageHandler.HandleIncomingMessage(msg, token);
                }
                else
                {
                    var genericRegistration = _container.GetRegistration(typeof(IMessageHandler));
                    if (genericRegistration != null)
                    {
                        var messageHandler = genericRegistration.GetInstance() as IMessageHandler;
                        await messageHandler.HandleIncomingMessage(msg, token);
                    }
                }
            }
        }
    }
}