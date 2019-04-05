namespace channels.usecase
{
    using System;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using channels.usecase.micro;

    class Program
    {
        static async Task Main(string[] args)
        {
            const string topicName1 = "t1";
            const string topicName2 = "t2";

            var instance = micro.MicroPubSub.GetInstance();

            await instance.InitTopic(topicName1);
            await instance.InitTopic(topicName2);

            hdlTopic("1", await instance.Sub(topicName1));
            hdlTopic("2", await instance.Sub(topicName2));
            hdlTopic("3", await instance.Sub(topicName2));

            for (int i = 0; i < 50; i++)
            {
                var messagedata = new Message<string>($"Message {i}");
                if ((i % 2) == 0)
                {
                    await instance.Pub(topicName1, messagedata);
                }
                else
                {
                    await instance.Pub(topicName2, messagedata);
                }
            }

            System.Threading.Thread.Sleep(100);
            instance.Dispose();

            async Task hdlTopic(string hdlName, ChannelReader<Message> cr) {
                while (await cr.WaitToReadAsync())
                {
                    if (cr.TryRead(out var message))
                        Console.WriteLine($"{hdlName} - receive: {message.Data}");
                }
            };
        }
    }

}
