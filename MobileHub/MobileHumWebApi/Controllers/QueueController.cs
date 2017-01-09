﻿using Microsoft.WindowsAzure.Storage.Queue;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Utilities;

namespace MobileHumWebApi.Controllers
{
    [RoutePrefix("api/queue")]
    public class QueueController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(string id)
        {
            var queue = new MobileAppCloudQueue(id);

            var msg = queue.GetMessage();
            if (msg != null)
            {
                queue.DeleteMessage(msg);
                return msg.AsString;
            }
            return string.Format("Queue [{0}] is Empty", id);

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
