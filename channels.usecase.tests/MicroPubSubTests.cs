namespace channels.usecase.tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using channels.usecase.micro;
    using System.Threading.Tasks;

    [TestClass]
    public class MicroPubSubTests
    {
        // testing a topic
        [TestMethod]
        public void InitialState_TopicsCountIsZero()
        {
            // Arrange
            // Act
            // Assert
            Assert.AreEqual(0, MicroPubSub.GetInstance().Topics);
        }

        [TestMethod]
        public async Task InitTopic_ValidTopicName_TopicsCountEqualsOne()
        {
            // Arrange
            const string topicName = "topic1";
            var instance = MicroPubSub.GetInstance();

            // Act
            await instance.InitTopic(topicName);

            // because of async channel`s nature, 
            // this time is needed to action run and sync new topic.
            System.Threading.Thread.Sleep(10);

            // Assert
            Assert.AreEqual(1, instance.Topics);
        }

        // testing pub to a topic
        [TestMethod]
        public async Task Pub_ValidTopicNameAndMessage_MessagePublished()
        {
            // Arrange
            const string topicName = "topic1";
            var instance = MicroPubSub.GetInstance();
            await instance.InitTopic(topicName);

            var message = new Message<string>("data");

            // Act
            var published = await instance.Pub(topicName, message);

            // Assert
            Assert.AreEqual(true, published);
        }

        [TestMethod]
        public async Task Pub_InvalidTopicName_TopicNameException()
        {
            // Arrange
            const string invalidTopicName = "";     
            const string topicName = "topic1";
            await MicroPubSub.GetInstance().InitTopic(topicName);

            var message = new Message<string>("data");      

            // Act
            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await MicroPubSub.GetInstance().Pub(invalidTopicName, message));
        }

        [TestMethod]
        public async Task Pub_InvalidMessage_MessageException()
        {
            // Arrange  
            const string topicName = "topic1";
            await MicroPubSub.GetInstance().InitTopic(topicName);  

            // Act
            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async ()  => await MicroPubSub.GetInstance().Pub(topicName, null));
        }

        // testing sub a topic
        [TestMethod]
        public async Task Sub_ValidTopicName_MessageConsumed()
        {
            // Arrange
            const string topicName = "topic1";
            var instance = MicroPubSub.GetInstance();
            await instance.InitTopic(topicName);

            var message = new Message<string>("data");

            var published = await instance.Pub(topicName, message);

            // Act
            var consumer = await instance.Sub(topicName);
            var hasMessage = consumer.TryRead(out var received);

            // Assert
            Assert.AreEqual(true, hasMessage);
            Assert.IsInstanceOfType(received, typeof(Message<string>));
            Assert.AreEqual(message.Data, received.Data);
        }

        [TestMethod]
        public async Task Sub_InvalidTopicName_TopicNameException()
        {
            // Arrange
            const string invalidTopicName = "";           

            // Act
            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => MicroPubSub.GetInstance().Sub(invalidTopicName));
        }
    }
}