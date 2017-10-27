using System;
using System.Collections.Generic;
using System.Text;
using Messaging.Transport.Abstractions.Consumer;
using System.Threading.Tasks;
using System.Threading;

namespace MediatR.Integration.Messaging.Transport
{
    public class GenericMediatRMessageHandler : IMessageHandler
    {
        private readonly IMediator _mediator;

        public GenericMediatRMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleIncomingMessage<TKey, TValue>(IMessage<TKey, TValue> message, CancellationToken token = default(CancellationToken))
        {
            var command = message.Value as IRequest;
            await _mediator.Send(command, token);
        }
    }
}