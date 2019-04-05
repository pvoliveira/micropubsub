namespace channels.usecase.micro
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public class MicroPubSub : IDisposable
    {
        private static MicroPubSub instance;

        private Channel<string> _addTopics;
        private ConcurrentDictionary<string, Channel<Message>> _queues;
        
        public int Topics => _queues.Keys.Count;

        public static MicroPubSub GetInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            instance = new MicroPubSub();

            return instance;
        }

        public MicroPubSub()
        {
            _addTopics = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() {
                SingleReader = true,
                SingleWriter = false
            });

            _queues = new ConcurrentDictionary<string, Channel<Message>>();
        }

        public async Task InitTopic(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Topic name can not be empty.", nameof(name));
            }

            if (_queues.Keys.Contains(name))
            {
                return;
            }
                    
            _queues.AddOrUpdate(
                name, 
                Channel.CreateUnbounded<Message>(new UnboundedChannelOptions() {
                    SingleReader = false,
                    SingleWriter = true
                }), 
                (k, av) => av);
        }

        public async ValueTask<bool> Pub(string topic, Message data)
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

        public async Task<ChannelReader<Message>> Sub(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Topic name can not be empty.", nameof(topic));
            }

            return await Task.FromResult(_queues.GetValueOrDefault(topic));       
        }

        public void Dispose()
        {
            if (_addTopics != null)
            {
                _addTopics.Writer.TryComplete();
            }

            foreach (var k in _queues.Keys)
            {
                if (_queues.TryRemove(k, out var ch))
                {
                    ch.Writer.Complete();
                }
            }
            
        }
    }
}