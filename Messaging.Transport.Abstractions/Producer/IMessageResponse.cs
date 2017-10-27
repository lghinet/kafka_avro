using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Producer
{
    public class MessageResponse : IMessageResponse
    {
        public bool Successful { set; get; }
        public string Response { set; get; }
        public int ResponseCode { set; get; }
        public string Request { get; set; }
    }

    public interface IMessageResponse
    {
        bool Successful { set; get; }
        string Response { set; get; }
        string Request { get; set; }
        int ResponseCode { set; get; }
    }

    public interface IBatchMessageResponse
    {
        int TotalCount { get; }
        bool Successful { set; get; }
        List<IMessageResponse> MessageResponses { get; }
    }

    public class BatchMessageResponse : IBatchMessageResponse
    {
        public List<IMessageResponse> MessageResponses { get; set; }
        public int TotalCount { get; set; }
        public bool Successful { get; set; }
    }
}
