using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Messaging.Transport.Abstractions.Producer;
using System.Threading.Tasks;

namespace Messaging.Transport.Kafka.Producer
{
    public class KafkaProducer : IProducer
    {
        private readonly IMessageConfigurationProvider<KafkaMessageConfiguration> _messageConfigProvider;
        private readonly ISchemaProducerFactory _schemaProducerFactory;

        public KafkaProducer(IMessageConfigurationProvider<KafkaMessageConfiguration> messageConfigProvider, ISchemaProducerFactory schemaProducerFactory)
        {
            _messageConfigProvider = messageConfigProvider;
            _schemaProducerFactory = schemaProducerFactory;
        }

        public Task<IMessageResponse> ProduceAsync<TPayload>(IMessage<TPayload> message)
        {
            return ProduceAsync<Null, TPayload>(null, message);
        }

        public async Task<IMessageResponse> ProduceAsync<TKey, TPayload>(TKey key, IMessage<TPayload> message)
        {
            var cfg = _messageConfigProvider.GetMessageConfiguration(message.MessageType);

            var producer = _schemaProducerFactory.GetSerializingProducer<TKey, TPayload>(cfg.Schema, cfg.Topic);
            var t = await producer.ProduceAsync(cfg.Topic, key, message.Data);

            return new MessageResponse
            {
                Successful = !t.Error.HasError,
                ResponseCode = (int) t.Error.Code,
                Response = t.Error.Reason
            };
        }

        public Task<IBatchMessageResponse> ProduceAsync<TPayload, TChunk>(IBatchMessage<TPayload, TChunk> message) where TPayload : class, IBatchData<TChunk>
        {
            return ProduceAsync<Null, TPayload, TChunk>(null, message);
        }

        public async Task<IBatchMessageResponse> ProduceAsync<TKey, TPayload, TChunk>(TKey key, IBatchMessage<TPayload, TChunk> message)
            where TPayload : class, IBatchData<TChunk>
        {
            var cfg = _messageConfigProvider.GetMessageConfiguration(message.MessageType);
            var tasks = new List<Task<IMessageResponse>>();
            var rows = new List<TChunk>(message.Data.Items);

            if (cfg.BatchSize > 1)
            {
                var pages = Math.Ceiling(rows.Count / Convert.ToDouble(cfg.BatchSize));
                for (var page = 0; page < pages; page++)
                {
                    message.Data.Items = rows.Skip(page * cfg.BatchSize).Take(cfg.BatchSize).ToList();
                    tasks.Add(ProduceAsync(key, (IMessage<TPayload>) message));
                }
            }
            else
            {
                foreach (var row in rows)
                {
                    message.Data.Items = new List<TChunk> {row};
                    tasks.Add(ProduceAsync(key, (IMessage<TPayload>) message));
                }
            }

            var responses = await Task.WhenAll(tasks);

            return new BatchMessageResponse {MessageResponses = responses.ToList(), TotalCount = rows.Count, Successful = responses.All(a => a.Successful)};
        }
    }
}