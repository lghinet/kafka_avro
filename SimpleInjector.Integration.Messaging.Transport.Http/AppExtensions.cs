using System;
using Messaging.Transport.Abstractions.Producer;
using Messaging.Transport.Http.Producer;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Messaging.Transport.Builder;

namespace SimpleInjector.Integration.Messaging.Transport.Http
{
    public static class AppExtensions
    {
        public static IMessagingBuilder WithHttpTransport(this IMessagingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (builder.Container == null)
                throw new ArgumentNullException(nameof(builder.Container));

            builder.Container.AppendToCollection(typeof(IProducer), typeof(HttpProducer));

            return builder;
        }
    }
}