using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using Common;
using Contracts;
using CustomerModel;
using Newtonsoft.Json;
using Utilities;


namespace MobileHumWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteQueueController : ApiController
    {
        readonly MobileAppCloudQueue _queue = new MobileAppCloudQueue(CommonConfigValues.ResponseQueueName);

        /// <inheritdoc />
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
        }

        // GET: table/User
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BestRouteContract Get()
        {
            var msg = _queue.GetMessage();

            if (msg == null) return null;

           // _queue.DeleteMessage(msg);

            return JsonConvert.DeserializeObject<BestRouteContract>(msg.AsString);
            
            



        }

    }
}
