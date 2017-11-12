using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace Utilities
{

    public class MobileAppCloudQueueAsync
    {
        private CloudQueueClient cloudQueueClient;
        private CloudQueue cloudQueue;

        public CloudQueue CloudQueue
        {
            get
            {
                return cloudQueue;
            }
        }

        public MobileAppCloudQueueAsync(string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExistsAsync().Wait();
        }
        
        public async void AddMessage(string message)
        {
            var msg = new CloudQueueMessage(message);
            await cloudQueue.AddMessageAsync(msg);
        }

        public async Task<CloudQueueMessage> GetMessage()
        {
            return await cloudQueue.GetMessageAsync();
        }

        public async Task DeleteMessage(CloudQueueMessage message)
        {
            
            if (message != null)
            {
                await cloudQueue.DeleteMessageAsync(message);
            }
        }
    }
}
