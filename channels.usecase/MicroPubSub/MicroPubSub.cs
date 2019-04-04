namespace channels.usecase.MicroPubSub
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
        private static ConcurrentDictionary<string, Channel<Message>> _topicsPub;
        
        public static IReadOnlyCollection<string> Topics => _topics;

        static MicroPubSub()
        {
            _addTopics = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() {
                SingleReader = true,
                SingleWriter = false
            });
            _topicsPub = new ConcurrentDictionary<string, Channel<Message>>();
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
                    _topicsPub.AddOrUpdate(topicName, Channel.CreateUnbounded<Message>(new UnboundedChannelOptions() {
                        SingleReader = false,
                        SingleWriter = false
                    }), 
                    (k, av) => av);
                    
                }
            };
        }

        public static async ValueTask<bool> InitTopic(string name)
        {
            while (await _addTopics.Writer.WaitToWriteAsync())
            {
                return await new ValueTask<bool>(_addTopics.Writer.TryWrite(name));                
            }

            return await new ValueTask<bool>(false);
        }

        public static async ValueTask<bool> Pub<T>(string topic, Message<T> data)
        {
            throw new NotImplementedException();
        }

        public static async Task<ChannelWriter<Message<T>>> Sub<T>(string topic)
        {
            throw new NotImplementedException();            
        }
    }
}