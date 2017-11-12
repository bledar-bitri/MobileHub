using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Utilities
{

    public class MobileAppCloudQueue
    {
        public CloudQueue CloudQueue { get; }

        public MobileAppCloudQueue(string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            CloudQueue = cloudQueueClient.GetQueueReference(queueName);
            CloudQueue.CreateIfNotExists();
        }
        
        public void AddMessage(string message)
        {
            var msg = new CloudQueueMessage(message);
            CloudQueue.AddMessage(msg);
        }

        public CloudQueueMessage GetMessage()
        {
            return CloudQueue.GetMessage();
        }

        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            return await CloudQueue.GetMessageAsync();
        }

        public void DeleteMessage(CloudQueueMessage message)
        {
            
            if (message != null)
            {
                CloudQueue.DeleteMessage(message);
            }

        }

        public ICancellableAsyncResult BeginGetMessage(AsyncCallback callback, object state)
        {
            return CloudQueue.BeginGetMessage(callback, state);
        }

        
    }
}
