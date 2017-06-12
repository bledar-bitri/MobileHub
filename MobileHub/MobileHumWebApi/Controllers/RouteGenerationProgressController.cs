using System.Web.Http;
using Common;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using Newtonsoft.Json;
using Utilities;

namespace MobileHumWebApi.Controllers
{
    [RoutePrefix("api/routeGenerationProgress")]
    public class RouteGenerationProgressController : ApiController
    {
        
        // GET api/values/5
        public Logging.QueueMessage Get()
        {
            var queue = new MobileAppCloudQueue(CommonConfigValues.ProgressLogQueueName);

            var msg = queue.GetMessage();

            if (msg == null) return null;

            queue.DeleteMessage(msg);

            return JsonConvert.DeserializeObject<Logging.QueueMessage>(msg.AsString);

        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
