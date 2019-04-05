namespace channels.usecase.micro
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public static class MicroPubSub
    {
        private static Channel<string> _addTopics;
        private static List<string> _topics;
        private static ConcurrentDictionary<string, Channel<Message>> _queues;
        
        public static IReadOnlyCollection<string> Topics => _topics;

        static MicroPubSub()
        {
            _addTopics = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() {
                SingleReader = true,
                SingleWriter = false
            });
            _queues = new ConcurrentDictionary<string, Channel<Message>>();
            _topics = new List<string>();

            handleAddTopics();

            async Task handleAddTopics()
            {
                while (await _addTopics.Reader.WaitToReadAsync())
                {
                    var topicName = await _addTopics.Reader.ReadAsync();
                    if (_topics.Contains(topicName))
                    {
                        continue;
                    }
                    _topics.Add(topicName);
                    _queues.AddOrUpdate(topicName, Channel.CreateUnbounded<Message>(new UnboundedChannelOptions() {
                        SingleReader = false,
                        SingleWriter = true
                    }), 
                    (k, av) => av);
                    
                }
            };
        }

        public static async ValueTask<bool> InitTopic(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Topic name can not be empty.", nameof(name));
            }

            while (await _addTopics.Writer.WaitToWriteAsync())
            {
                return await new ValueTask<bool>(_addTopics.Writer.TryWrite(name));                
            }

            return await new ValueTask<bool>(false);
        }

        public static async ValueTask<bool> Pub(string topic, Message data)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Topic name can not be empty.", nameof(topic));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (_queues.TryGetValue(topic, out var ch))
            {
                return await new ValueTask<bool>(ch.Writer.TryWrite(data));
            }

            return await new ValueTask<bool>(false);
        }

        public static async Task<ChannelReader<Message>> Sub(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Topic name can not be empty.", nameof(topic));
            }

            return await Task.FromResult(_queues.GetValueOrDefault(topic));       
        }
    }
}