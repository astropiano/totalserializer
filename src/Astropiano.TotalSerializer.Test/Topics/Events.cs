using System.ComponentModel;
using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.Test.TestCases;
using Astropiano.TotalSerializer.Test.TestCases.Array;
using Astropiano.TotalSerializer.Test.TestCases.Events;
using Astropiano.TotalSerializer.Test.TestCases.Notifying;

namespace Astropiano.TotalSerializer.Test.Topics
{
    public class Events
    {
        [Fact]
        public void PropertyChangedEventTest()
        {
            TestClass17PropertyChangedReceiver receiver = new TestClass17PropertyChangedReceiver();
            TestClass16PropertyChangedSender sender = new TestClass16PropertyChangedSender();
            
            receiver.ListenToChanges(sender);
            receiver.SenderInstance = sender; //so we can access the instance after deserialization
            
            TotalSerializer totalSerializer = new TotalSerializer();
            string json = totalSerializer.Serialize(receiver);

            TestClass17PropertyChangedReceiver receiverDeserialized =
                totalSerializer.Deserialize<TestClass17PropertyChangedReceiver>(json);

            receiverDeserialized.ChangedEvendHasFired.Should().BeFalse();
            
            receiverDeserialized.SenderInstance.TriggerChangedEvent("test");
            
            receiverDeserialized.ChangedEvendHasFired.Should().BeTrue();
        }

        [Fact]
        public void MultiPropertyChangedEventRegistration()
        {
            TestClass17PropertyChangedReceiver receiver = new TestClass17PropertyChangedReceiver();
            TestClass17PropertyChangedReceiver receiver2 = new TestClass17PropertyChangedReceiver();
            TestClass16PropertyChangedSender sender = new TestClass16PropertyChangedSender();
            
            receiver.ListenToChanges(sender);
            receiver2.ListenToChanges(sender);
            receiver.SenderInstance = sender; //so we can access the instance after deserialization
            receiver.AnotherReceiver = receiver2; //so we can access the instance after deserialization
            
            TotalSerializer totalSerializer = new TotalSerializer();
            string json = totalSerializer.Serialize(receiver);
            
            TestClass17PropertyChangedReceiver receiverDeserialized =
                totalSerializer.Deserialize<TestClass17PropertyChangedReceiver>(json);
            TestClass17PropertyChangedReceiver receiverDeserialized2 = receiverDeserialized.AnotherReceiver;
            
            receiverDeserialized.ChangedEvendHasFired.Should().BeFalse();
            receiverDeserialized2.ChangedEvendHasFired.Should().BeFalse();
            receiverDeserialized.Count.Should().Be(0);
            receiverDeserialized2.Count.Should().Be(0);
            
            receiverDeserialized.SenderInstance.TriggerChangedEvent("test");
            
            receiverDeserialized.ChangedEvendHasFired.Should().BeTrue();
            receiverDeserialized2.ChangedEvendHasFired.Should().BeTrue();
            receiverDeserialized.Count.Should().Be(1);
            receiverDeserialized2.Count.Should().Be(1);
        }

        [Fact]
        public void AnonymousMethodTest()
        {
            TestClass16PropertyChangedSender sender = new TestClass16PropertyChangedSender();
            TestClass20WithAnonymousEventSubscription receiver = new TestClass20WithAnonymousEventSubscription();
            receiver.SubScribeTo(sender);

            receiver.TreeReference = sender;
            
            TotalSerializer totalSerializer = new TotalSerializer();
            string json = totalSerializer.Serialize(receiver);
            
            receiver.ReceivedEvent.Should().BeFalse();
            receiver.Count.Should().Be(0);
            
            TestClass20WithAnonymousEventSubscription receiverDeserialized =
                totalSerializer.Deserialize<TestClass20WithAnonymousEventSubscription>(json);
            TestClass16PropertyChangedSender senderD = receiverDeserialized.TreeReference;
            
            receiverDeserialized.ReceivedEvent.Should().BeFalse();
            receiverDeserialized.Count.Should().Be(0);
            senderD.TriggerChangedEvent("test");
            receiverDeserialized.ReceivedEvent.Should().BeTrue();
            receiverDeserialized.Count.Should().Be(1);
        }

        [Fact]
        public void EventHandlersInArrayTest()
        {
            TestClass16PropertyChangedSender subObj = new TestClass16PropertyChangedSender();
            TestClass13WithObject obj = new TestClass13WithObject();
            obj.Obj = subObj;
            TestClass11WithObjectArray subObj2 = new TestClass11WithObjectArray();
            
            PropertyChangedEventHandler changedEventHandler = (sender, args) =>
            {
                obj.BooleanValue = true;
            };
            
            subObj2.SetObjectArray(new PropertyChangedEventHandler[]{changedEventHandler});

            obj.Obj2 = subObj2;

            subObj.PropertyChanged += changedEventHandler;
            
            TotalSerializer totalSerializer = new TotalSerializer();
            string json = totalSerializer.Serialize(obj);

            TestClass13WithObject objD = totalSerializer.Deserialize<TestClass13WithObject>(json);

            TestClass16PropertyChangedSender subObjD = objD.Obj as TestClass16PropertyChangedSender;
            subObjD.Should().NotBeNull();
            TestClass11WithObjectArray subObj2D = objD.Obj2 as TestClass11WithObjectArray;
            subObj2D.Should().NotBeNull();
            
            subObjD.TriggerChangedEvent("test");
            
            obj.BooleanValue.Should().BeFalse();
            objD.BooleanValue.Should().BeTrue();

            objD.BooleanValue = false;

            PropertyChangedEventHandler[] array = subObj2D.GetObjectArray() as PropertyChangedEventHandler[];
            array.Should().NotBeNull();
            subObjD.PropertyChanged -= array[0];
            
            subObjD.TriggerChangedEvent("test");

            objD.BooleanValue.Should().BeFalse();
        }

        /// <summary>
        /// This test case describes a class of problems where you have an event handler. The anonymous method might actually live somewhere else
        /// causing a challenge for the serialization...
        /// CURRENTLY THIS DOES NOT WORK
        /// </summary>
        [Fact]
        public void PropertyChangedHandlerTest()
        {
            TestClass25WithPropertyChangeHandlerManager testCase1 = new TestClass25WithPropertyChangeHandlerManager();
            TestClass25WithPropertyChangeHandlerManager testCase1Other = new TestClass25WithPropertyChangeHandlerManager();
            testCase1.other = testCase1Other;

            testCase1.PropertyChangedHandlerManager.AddPropertyChangedHandler(testCase1Other, nameof(TestClass25WithPropertyChangeHandlerManager.Name),
                () =>
                {
                    testCase1.ChangedCount++;
                }
            );

            testCase1.ChangedCount.Should().Be(0);

            testCase1Other.Name = "one";

            testCase1.ChangedCount.Should().Be(1);

            TotalSerializer serializer = new TotalSerializer();

            string json = serializer.Serialize(testCase1);

            TestClass25WithPropertyChangeHandlerManager testCase2 = serializer.Deserialize<TestClass25WithPropertyChangeHandlerManager>(json);

            testCase2.ChangedCount.Should().Be(1);

            TestClass25WithPropertyChangeHandlerManager testCase2Other = testCase2.other;

            testCase2Other.Name = "two";

            testCase2.ChangedCount.Should().Be(2); // for case2 the value should change

            testCase1.ChangedCount.Should().Be(1); // for case1 the value should not have changed
        }
    }
}