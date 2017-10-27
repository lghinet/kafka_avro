using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Messaging.Transport.Abstractions.Producer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Messaging.Transport.Http.Producer
{
    public class HttpProducer : IProducer
    {
        private readonly IConfigurationRoot _config;
        private readonly IMessageConfigurationProvider<HttpMessageConfiguration> _messageConfigProvider;

        public HttpProducer(IConfigurationRoot configuration, IMessageConfigurationProvider<HttpMessageConfiguration> messageConfigProvider)
        {
            _config = configuration;
            _messageConfigProvider = messageConfigProvider;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.SetBearerToken(RequestToken());

            return client;
        }

        private string RequestToken()
        {
            var client = new TokenClient(_config.GetSection("SSOConfig").GetValue<string>("TokenEndpoint"),
                _config.GetSection("SSOConfig").GetValue<string>("ClientId"),
                _config.GetSection("SSOConfig").GetValue<string>("ClientSecret"));
            var a = client.RequestClientCredentialsAsync(_config.GetSection("SSOConfig").GetValue<string>("Scopes")).Result;
            return a.AccessToken;
        }

        public async Task<IMessageResponse> ProduceAsync<TPayload>(IMessage<TPayload> message)
        {
            var cfg = _messageConfigProvider.GetMessageConfiguration(message.MessageType);

            var messOut = new MessageResponse();
            using (var client = GetHttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(message.Data), Encoding.UTF8, "application/json");
                var resp = await client.PostAsync(cfg.Endpoint, httpContent);

                messOut.Successful = (int) resp.StatusCode == 200;
                messOut.ResponseCode = (int) resp.StatusCode;
                messOut.Response = await resp.Content.ReadAsStringAsync();
            }

            return messOut;
        }

        public Task<IMessageResponse> ProduceAsync<TKey, TPayload>(TKey key, IMessage<TPayload> message)
        {
            return ProduceAsync(message);
        }

        public async Task<IBatchMessageResponse> ProduceAsync<TPayload, TChunk>(IBatchMessage<TPayload, TChunk> message)
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
                    tasks.Add(ProduceAsync((IMessage<TPayload>) message));
                }
            }
            else
            {
                foreach (var row in rows)
                {
                    message.Data.Items = new List<TChunk> {row};
                    tasks.Add(ProduceAsync((IMessage<TPayload>) message));
                }
            }

            var responses = await Task.WhenAll(tasks);

            return new BatchMessageResponse {MessageResponses = responses.ToList(), TotalCount = rows.Count, Successful = responses.All(a => a.Successful)};
        }

        public Task<IBatchMessageResponse> ProduceAsync<TKey, TPayload, TChunk>(TKey key, IBatchMessage<TPayload, TChunk> message)
            where TPayload : class, IBatchData<TChunk>
        {
            return ProduceAsync(message);
        }
    }
}