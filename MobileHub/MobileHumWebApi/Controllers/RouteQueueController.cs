using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Common;
using Contracts;
using CustomerModel;
using Newtonsoft.Json;
using Utilities;


namespace MobileHumWebApi.Controllers
{
    public class RouteQueueController : ApiController
    {
        readonly MobileAppCloudQueue queue = new MobileAppCloudQueue(CommonConfigValues.ResponseQueueName);

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
        }

        // GET: table/User
        public IEnumerable<CityContract> Get()
        {
            var msg = queue.GetMessage();

            if (msg == null) return null;

           // queue.DeleteMessage(msg);

            return JsonConvert.DeserializeObject<List<CityContract>>(msg.AsString);
            
            



        }

    }
}
