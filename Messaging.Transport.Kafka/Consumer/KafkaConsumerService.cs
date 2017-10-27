using Confluent.Kafka;
using Messaging.Transport.Abstractions.Consumer;
using Messaging.Transport.Abstractions.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging.Transport.Kafka.Consumer
{
    public class KafkaConsumerService : IConsumerService
    {
        private readonly CancellationTokenSource _callbackCts;
        private bool _cancelled;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly ConsumerServiceOptions _options;
        private readonly ISchemaConsumerFactory _consumerFactory;
        private readonly List<IConsumer> _consumers;
        private readonly SingleInstanceFactory _factory;

        public KafkaConsumerService(ConsumerServiceOptions options, ILogger<KafkaConsumerService> logger, ISchemaConsumerFactory consumerFactory,
            SingleInstanceFactory factory)
        {
            _callbackCts = new CancellationTokenSource();
            _cancelled = false;
            _logger = logger;
            _factory = factory;
            _options = options;
            _consumerFactory = consumerFactory;

            _consumers = new List<IConsumer>();

            //var actionBlockOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 10, CancellationToken = _callbackCts.Token };
            //_workerBlock = new ActionBlock<Message<TKey, TValue>>(async item => await OnMessageReceivedAsync(item, _callbackCts.Token), actionBlockOptions);
        }

        public IConsumerService AddConsumer<TKey, TValue>(Func<IMessage<TKey, TValue>, CancellationToken, Task> onMessageReceived = null)
        {
            foreach (var opt in _options.Topics.Where(x => x.Command == typeof(TValue).FullName).GroupBy(a => a.Schema))
                InternalAddConsumer(opt.Key, opt.Select(a => a.Topic).ToList(), onMessageReceived);

            return this;
        }

        private void InternalAddConsumer<TKey, TValue>(string schema, List<string> topics, Func<IMessage<TKey, TValue>, CancellationToken, Task> callback)
        {
            var consumer = _consumerFactory.GetSerializingConsumer<TKey, TValue>(schema);
            _consumers.Add(consumer);

            // Note: All event handlers are called on the main thread.

            consumer.OnMessage += (_, msg) => OnMessageReceived(msg, callback);
            consumer.OnError += (_, error) => Console.WriteLine($"Error: {error}");

            consumer.OnPartitionsAssigned += (_,partitions) =>
            {
                Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                consumer.Assign(partitions);
            };

            consumer.OnConsumeError += (_, msg)
                => _logger.LogError($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

            consumer.Subscribe(topics);

            _logger.LogInformation($"Subscribed to: [{string.Join(", ", consumer.Subscription)}]");
        }

        public virtual Task Start()
        {
            var tasks = new List<Task>();

            foreach (var consumer in _consumers)
                tasks.Add(InternalStart(consumer));

            return Task.WhenAll(tasks);
        }

        protected virtual async Task InternalStart(IConsumer consumer)
        {
            await Task.Run(() =>
            {
                while (!_cancelled)
                {
                    consumer.Poll(TimeSpan.FromMilliseconds(100));
                }
            }, _callbackCts.Token);
        }

        public void Stop()
        {
            _callbackCts.Cancel();
            _cancelled = true;
        }

        private void OnMessageReceived<TKey, TValue>(Message<TKey, TValue> msg, Func<IMessage<TKey, TValue>, CancellationToken, Task> callback)
        {
            var context = _factory(typeof(IMessageContextHandler<TKey, TValue>)) as IMessageContextHandler<TKey, TValue>;
            if (context == null)
                throw new NullReferenceException($"IMessageContextHandler not implemented for type {typeof(TValue)}");

            var newMessage = new KafkaMessage<TKey, TValue>(msg);
            context.HandleIncomingMessageContext(newMessage, _callbackCts.Token)
                .ContinueWith(task => callback?.Invoke(newMessage, _callbackCts.Token))
                .Wait();

            _logger.LogInformation($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
        }

        public void Dispose()
        {
            foreach (var cons in _consumers)
            {
                try
                {
                    cons.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}