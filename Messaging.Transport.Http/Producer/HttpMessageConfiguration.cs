using System;
using System.Collections.Generic;
using Messaging.Transport.Abstractions.Producer;

namespace Messaging.Transport.Http.Producer
{
    public class HttpMessageConfiguration : IMessageConfiguration
    {
        public string MessageType { set; get; }
        public int BatchSize { get; set; }
        public string Endpoint { set; get; }

        public void Init(IDictionary<string, string> settings)
        {
            Endpoint = settings["Endpoint"];
            MessageType = settings["MessageType"];
            BatchSize = settings.ContainsKey("BatchSize") ? Convert.ToInt32(settings["BatchSize"]) : 1;
        }
    }
}