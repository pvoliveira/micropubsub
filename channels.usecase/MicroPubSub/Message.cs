namespace channels.usecase.MicroPubSub
{
    using System;

    public abstract class Message
    {
        private string _topic;
        private string _metadata;
        private object _data;

        public string Topic => _topic;

        public Message(object data)
        {
            _data = data;
        }

        public Message(string[] metadata, object data)
        {
            _metadata = string.Join(';', metadata);
            _data = data;
        }

        public 
    }

    public class Message<T> : Message
    {
        public Message(T data) : base(data)
        {
        }

        public Message(string[] metadata, T data) : base(metadata, data)
        {
        }


    }

    // public ref struct Message<T>
    // {
    //     public ReadOnlySpan<char> Metatada { get; }
    //     public string Topic { get; }
    //     public T Data { get; }

    //     public Message(ReadOnlySpan<char> metadata, string topic, T data)
    //     {
    //         this.Metatada = metadata;
    //         this.Topic = topic;
    //         this.Data = data;
    //     }
    // }
}