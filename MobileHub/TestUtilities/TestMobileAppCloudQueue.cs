using Newtonsoft.Json;
using System.Diagnostics;
using Utilities;

namespace TestUtilities
{
    public class TestMobileAppCloudQueue
    {
        public static string QueueName = "test";

        public TestMobileAppCloudQueue()
        {
            for (int i = 0; i < 10; i++)
            {
                TestSendMessage();
            }
            TestReadMessages();
        }
        public static void TestSendMessage()
        {
            var queue = new MobileAppCloudQueue(QueueName);

            queue.AddMessage(JsonConvert.SerializeObject(new Person() {FirstName = "Tester", LastName = "Test" }));
        }

        public static void TestReadMessages()
        {
            var queue = new MobileAppCloudQueue(QueueName);
            var msg = queue.GetMessage();
            while (msg != null)
            {
                if (msg == null) break;
                Trace.TraceInformation(string.Format("message: {0}", msg.AsString));
                var person = JsonConvert.DeserializeObject<Person>(msg.AsString);
                Trace.TraceInformation(string.Format("PERSON: {0}", person.ToString()));
                queue.DeleteMessage(msg);
                msg = queue.GetMessage();
            }
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return string.Format("FirstName: {0}  LastName: {1}", FirstName, LastName);
        }
    }
}
