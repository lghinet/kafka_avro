using Messaging.Transport.Abstractions.Consumer;
using Messaging.Transport.Abstractions.Core;
using Messaging.Transport.Abstractions.Producer;
using SimpleInjector.Integration.Messaging.Transport.Builder;
using SimpleInjector.Integration.Messaging.Transport.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleInjector.Integration.Messaging.Transport
{
    public static class AppExtensions
    {
        public static IMessagingBuilder EnableMessaging(this Container container, Action<MessagingOptions> options)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var builder = new MessagingBuilder(container);

            builder.Container.RegisterSingleton(new SingleInstanceFactory(builder.Container.GetInstance));
            builder.Container.RegisterSingleton(new MultiInstanceFactory(builder.Container.GetAllInstances));

            var opt = new MessagingOptions(container);
            options(opt);

            builder.HasConsumer = opt.HasConsumer;
            builder.HasProducer = opt.HasProducer;

            return builder;
        }

        public static void AddProducer(this MessagingOptions builder, Action<ProducerConfig> configuration)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = new ProducerConfig();
            configuration(config);

            if (config.ProducerConfiguration == null)
                throw new ArgumentNullException(nameof(config.ProducerConfiguration));

            builder.Container.Register(typeof(IEnumerable<MessageConfiguration>), () => config.MessageConfigurations.Select(a =>
            {
                var c = new MessageConfiguration();
                c.Init(a);
                return c;
            }), Lifestyle.Singleton);

            builder.Container.Register(typeof(IMessageConfigurationProvider<>), typeof(MessageConfigurationProvider<>), Lifestyle.Singleton);
            builder.Container.RegisterSingleton<IProducerFactory, ProducerFactory>();
            builder.Container.RegisterSingleton<IProducerConfiguration>(config);

            builder.HasProducer = true;
        }

        public static void AddConsumer(this MessagingOptions builder, Action<ConsumerConfig> configuration)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = new ConsumerConfig();
            // standard options
            configuration(options);

            if (options.ConsumerConfiguration == null)
                throw new ArgumentNullException(nameof(options.ConsumerConfiguration));

            foreach (var topic in options.Topics)
            {
                topic.Schema = topic.Schema ?? "Default";
            }

            //builder.Container.RegisterSingleton<ITopicCommandResolver>(topicCommandResolver);
            builder.Container.Register(typeof(IMessageContextHandler<,>), typeof(GenericMessageContext<,>), Lifestyle.Singleton);

            builder.Container.RegisterSingleton(new ConsumerServiceOptions { Topics = options.Topics });
            builder.Container.RegisterSingleton<IConsumerConfiguration>(options);

            builder.HasConsumer = true;
        }
    }
}