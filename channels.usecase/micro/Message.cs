namespace channels.usecase.micro
{
    using System;

    public abstract class Message
    {
        private string _topic;
        private string _metadata;
        private object _data;

        public string Topic => _topic;

        public object Data => _data;

        public Message(object data)
        {
            _data = data;
        }

        public Message(string[] metadata, object data)
        {
            _metadata = string.Join(';', metadata);
            _data = data;
        }
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
}