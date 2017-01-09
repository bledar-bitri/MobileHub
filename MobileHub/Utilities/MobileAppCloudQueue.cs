using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading.Tasks;

namespace Utilities
{

    public class MobileAppCloudQueue
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

        public MobileAppCloudQueue(string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            cloudQueue = cloudQueueClient.GetQueueReference(queueName);
            cloudQueue.CreateIfNotExists();
        }
        
        public void AddMessage(string message)
        {
            var msg = new CloudQueueMessage(message);
            cloudQueue.AddMessage(msg);
        }

        public CloudQueueMessage GetMessage()
        {
            return cloudQueue.GetMessage();
        }

        public void DeleteMessage(CloudQueueMessage message)
        {
            
            if (message != null)
            {
                cloudQueue.DeleteMessage(message);
            }
        }
    }
}
