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
            Assert.AreEqual(0, MicroPubSub.Topics.Count);
        }

        [TestMethod]
        public async Task InitTopic_ValidTopicName_TopicsCountEqualsOne()
        {
            // Arrange
            const string topicName = "topic1";
            // Act
            var topicIsCreated = await MicroPubSub.InitTopic(topicName);

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
        public async Task Pub_ValidTopicNameAndMessage_MessagePublished()
        {
            // Arrange
            const string topicName = "topic1";
            await MicroPubSub.InitTopic(topicName);

            var message = new Message<string>("data");

            // Act
            var published = await MicroPubSub.Pub(topicName, message);

            // Assert
            Assert.AreEqual(true, published);
        }

        [TestMethod]
        public async Task Pub_InvalidTopicName_TopicNameException()
        {
            // Arrange
            const string invalidTopicName = "";     
            const string topicName = "topic1";
            await MicroPubSub.InitTopic(topicName);

            var message = new Message<string>("data");      

            // Act
            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await MicroPubSub.Pub(invalidTopicName, message));
        }

        [TestMethod]
        public async Task Pub_InvalidMessage_MessageException()
        {
            // Arrange  
            const string topicName = "topic1";
            await MicroPubSub.InitTopic(topicName);  

            // Act
            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async ()  => await MicroPubSub.Pub(topicName, null));
        }

        // testing sub a topic
        [TestMethod]
        public async Task Sub_ValidTopicName_MessageConsumed()
        {
            // Arrange
            const string topicName = "topic1";
            await MicroPubSub.InitTopic(topicName);

            var message = new Message<string>("data");

            var published = await MicroPubSub.Pub(topicName, message);            

            // Act
            var consumer = await MicroPubSub.Sub(topicName);
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
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => MicroPubSub.Sub(invalidTopicName));
        }

    }
}