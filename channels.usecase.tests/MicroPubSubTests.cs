/*
Some concepts about this micro pub/sub:
- Topic: a channel which is created in runtime and assign to a type of data. (e.g: InitTopic<T>(string topicName) returns a ChannelWriter<T> )
*/
namespace channels.usecase.tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using channels.usecase.MicroPubSub;
    using System.Threading.Tasks;

    [TestClass]
    public class MicroPubSubTests
    {
        // state
        

        // testing a topic
        [TestMethod]
        public void MicroPubSub_StartState_NoExceptions()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0, MicroPubSub.Topics.Count);
        }

        [TestMethod]
        public async Task MicroPubSub_InitTopic_NewPublisherInstance()
        {
            // Arrange
            const string topicName = "topic1";
            // Act
            var topicIsCreated = await MicroPubSub.InitTopic(name: topicName);

            // because of async channel`s nature, 
            // this time is needed to action run and sync new topic.
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.IsNotNull(topicIsCreated);
            Assert.AreEqual(true, topicIsCreated);
            Assert.AreEqual(1, MicroPubSub.Topics.Count);
        }

        // testing pub to a topic
        [TestMethod]
        public async Task MicroPubSub_PublishMessageOnTopic_NoExceptions()
        {
            
        }

        // testing sub a topic


    }
}