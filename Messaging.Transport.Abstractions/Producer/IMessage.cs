using System;
using System.Collections.Generic;

namespace Messaging.Transport.Abstractions.Producer
{
    public interface IMessage<T>
    {
        string Id { set; get; }
        string MessageType { set; get; }
        T Data { set; get; }
    }

    public interface IBatchData<T>
    {
        List<T> Items { set; get; }
    }

    public interface IBatchMessage<TData, TRow> : IMessage<TData> where TData : class, IBatchData<TRow>
    {
    }

    public class Message<T> : IMessage<T>
    {
        public string Id { set; get; }
        public string MessageType { set; get; }
        public T Data { get; set; }
    }

    public class BatchMessage<TData, TRow> : IBatchMessage<TData, TRow> where TData : class, IBatchData<TRow>
    {
        public string Id { set; get; }
        public string MessageType { set; get; }
        public TData Data { get; set; }
    }
}