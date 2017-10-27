using System;
using System.Collections;
using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Producer
{
    public class MessageConfiguration : IMessageConfiguration
    {
        public IDictionary<string, string> Settings { get; set; }
        public string MessageType { set; get; }
        public string TransportType { set; get; }
        public int BatchSize { set; get; }

        public void Init(IDictionary<string, string> settings)
        {
            MessageType = settings["MessageType"];
            TransportType = settings["TransportType"];
            BatchSize = settings.ContainsKey("BatchSize") ? Convert.ToInt32(settings["BatchSize"]) : 1;
            Settings = settings;
        }
    }
}