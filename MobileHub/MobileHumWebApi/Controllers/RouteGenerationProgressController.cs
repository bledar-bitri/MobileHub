using System.Web.Http;
using Common;
using Logging;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using Newtonsoft.Json;
using Utilities;

namespace MobileHumWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/routeGenerationProgress")]
    public class RouteGenerationProgressController : ApiController
    {
        
        // GET api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProgressQueueMessage Get()
        {
            var queue = new MobileAppCloudQueue(CommonConfigValues.ProgressLogQueueName);

            var msg = queue.GetMessage();

            if (msg == null) return null;

            queue.DeleteMessage(msg);

            return JsonConvert.DeserializeObject<ProgressQueueMessage>(msg.AsString);

        }

        // POST api/values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
        }
    }
}
