using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Judo.SchemaRegistryClient;
using Messaging.Transport.Kafka.Avro.Surrogates;
using Messaging.Transport.Kafka.Serialization;
using Microsoft.Hadoop.Avro;

namespace Messaging.Transport.Kafka.Avro.Serialization
{
    public class SchemaRegistryAvroSerializer : IKafkaSerializer
    {
        private readonly ISchemaRegistryClient _schemaRegistryClient;
        private readonly ConcurrentDictionary<Type, object> _serializerCache = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, object> _deserializerCache = new ConcurrentDictionary<Type, object>();
        private readonly bool _useAvroDataContractResolver;

        public SchemaRegistryAvroSerializer(ISchemaRegistryClient schemaRegistryClient, bool useAvroDataContractResolver = false)
        {
            _schemaRegistryClient = schemaRegistryClient;
            _useAvroDataContractResolver = useAvroDataContractResolver;
        }

        public async Task<TPayload> DeserializeAsync<TPayload>(byte[] payload)
        {
            using (var stream = new MemoryStream(payload))
            {
                var buf = new byte[sizeof(uint)];
                stream.Seek(sizeof(byte), SeekOrigin.Begin);
                stream.Read(buf, 0, sizeof(uint));

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buf);

                var uintSchemaId = BitConverter.ToUInt32(buf, 0);
                var schema = await _schemaRegistryClient.GetByIDAsync((int) uintSchemaId);
                var serializer = GetDeserializer<TPayload>(schema.ToString());

                //stream.Seek(sizeof(byte) + sizeof(uint), SeekOrigin.Begin);
                return serializer.Deserialize(stream);
            }
        }

        public async Task<byte[]> SerializeAsync<TPayload>(TPayload payload, bool isKey, string topic)
        {
            var subject = GetSubjectName(topic, isKey);
            var serializer = GetSerializer<TPayload>();

            var schemaId = await _schemaRegistryClient.RegisterAsync(subject, serializer.ReaderSchema);
            var uintSchemaId = Convert.ToUInt32(schemaId);
            if (BitConverter.IsLittleEndian)
            {
                uintSchemaId = SwapEndianness(uintSchemaId);
            }

            using (var stream = new MemoryStream())
            {
                var sw = new BinaryWriter(stream);
                sw.Write((byte) 0x0);
                sw.Write(uintSchemaId);
                sw.Flush();
                serializer.Serialize(stream, payload);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        private IAvroSerializer<TPayload> GetDeserializer<TPayload>(string writeSchema)
        {
            var serializer = (IAvroSerializer<TPayload>)_deserializerCache.GetOrAdd(typeof(TPayload),
                type => AvroSerializer.CreateDeserializerOnly<TPayload>(writeSchema, new AvroSerializerSettings
                {
                    Resolver = new ContractResolver.AvroDataContractResolver(true),
                    //_useAvroDataContractResolver
                    //    ? (AvroContractResolver) new AvroDataContractResolver(false)
                    //    : new AvroPublicMemberContractResolver(false),
                    Surrogate = new AvroSurrogateStrategy(),
                    GenerateDeserializer = true,
                    GenerateSerializer = false
                }));

            return serializer;
        }

        private IAvroSerializer<TPayload> GetSerializer<TPayload>()
        {
            var serializer = (IAvroSerializer<TPayload>) _serializerCache.GetOrAdd(typeof(TPayload),
                type => AvroSerializer.Create<TPayload>(new AvroSerializerSettings
                {
                    Resolver = new ContractResolver.AvroDataContractResolver(true),
                    //_useAvroDataContractResolver
                    //    ? (AvroContractResolver) new AvroDataContractResolver(false)
                    //    : new AvroPublicMemberContractResolver(false),
                    Surrogate = new AvroSurrogateStrategy(),
                    GenerateDeserializer = false,
                    GenerateSerializer = true
                }));

            return serializer;
        }

        private string GetSubjectName(string topic, bool isKey)
        {
            return topic + (isKey ? "-key" : "-value");
        }

        uint SwapEndianness(uint x)
        {
            return ((x & 0x000000ff) << 24) + // First byte
                   ((x & 0x0000ff00) << 8) + // Second byte
                   ((x & 0x00ff0000) >> 8) + // Third byte
                   ((x & 0xff000000) >> 24); // Fourth byte
        }
    }
}