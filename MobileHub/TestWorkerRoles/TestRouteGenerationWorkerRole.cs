using Newtonsoft.Json;
using Parameters;
using System;
using Utilities;

namespace TestWorkerRoles
{
    public class TestRouteGenerationWorkerRole
    {
        public static string QueueName = "routegeneration";
        private static MobileAppCloudQueue queue;
        public TestRouteGenerationWorkerRole()
        {
            queue = new MobileAppCloudQueue(QueueName);

            string userId;
            Console.WriteLine("Testing Route Generation Enter UserId: (-1 to exit)");
            userId = Console.ReadLine();
            while (userId.Trim() != "-1")
            {
                try
                {
                    int paramUserId = Convert.ToInt32(userId);
                    for(int i =0; i < paramUserId; i++)
                    SendMessage(i);
                    Console.WriteLine("\n\tEnter another UserId: (-1 to exit)");
                    userId = Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Error while reading input: {0}", e.Message));
                    Console.ReadLine();
                    userId = "-1";
                }
            }
        }
        public static void SendMessage(int userId)
        {
            var param = new RouteRequestParameters { UserId = userId };
            Console.WriteLine("Sending Parameter into the queue: {0}", param);
            queue.AddMessage(JsonConvert.SerializeObject(param));
        }
    }
}
