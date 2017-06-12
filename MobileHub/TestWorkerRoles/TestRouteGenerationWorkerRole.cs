using Newtonsoft.Json;
using Parameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Contracts;
using Utilities;

namespace TestWorkerRoles
{
    public class TestRouteGenerationWorkerRole
    {
        public static string QueueName = "routegeneration";
        
        private static MobileAppCloudQueue queue;
        public TestRouteGenerationWorkerRole()
        {

            //Task.Factory.StartNew(MonitorResponseQueue);
            //Task.Factory.StartNew(ClearResponseQueue);

            queue = new MobileAppCloudQueue(QueueName);

            string userId;
            Console.WriteLine("Testing Route Generation Enter UserId: (-1 to exit)");
            userId = Console.ReadLine();
            while (userId.Trim() != "-1")
            {
                try
                {
                    int paramUserId = Convert.ToInt32(userId);
                    
                    //for(int i =0; i < paramUserId; i++)
                    //SendMessage(i);
                    SendMessage(paramUserId);

                    Console.WriteLine("\n\tEnter another UserId: (-1 to exit)");
                    userId = Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while reading input: {e.Message}");
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

        public static async void MonitorResponseQueue()
        {
            var responseQueue = new MobileAppCloudQueue(CommonConfigValues.ResponseQueueName);
            while (true)
            {
                //Console.WriteLine("Working");
                await Task.Delay(1000);

                var msg = responseQueue.GetMessage();

                if (msg == null) continue;

                var cities = JsonConvert.DeserializeObject<List<CityContract>>(msg.AsString);

                Console.WriteLine("Found Tour\n");
                cities.ForEach(c => Console.WriteLine($"{c.Id}: {c.Name}"));
                Console.WriteLine("\nEnd of Tour\n");

                responseQueue.DeleteMessage(msg);
            }
        }


        public static async void ClearResponseQueue()
        {
            var responseQueue = new MobileAppCloudQueue(CommonConfigValues.ResponseQueueName);
            while (true)
            {
                await Task.Delay(60000);
                Console.WriteLine("clearing queue");
                

                var msg = responseQueue.GetMessage();


                while (msg != null)
                {
                    responseQueue.DeleteMessage(msg);
                    msg = responseQueue.GetMessage();
                }

                Console.WriteLine("queue ist clear");
            }
        }

    }
}
