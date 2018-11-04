using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using CustomerModel;
using Microsoft.Azure.Mobile.Server;
using Services;


namespace MobileHumWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressController : TableController<Address>
    {
        /// <inheritdoc />
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
            DomainManager = new EntityDomainManager<Address>(context, Request);
        }

        // GET: table/User
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Address> Get()
        {
            using (var service = new AddressService())
            {
                return service.GetAllAddresses();
            }
        }

        // GET tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Address GetAddress(string id)
        {
            using (var service = new AddressService())
            {
                return service.GetAddress(id);
            }
        }


        // PATCH tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        public Task<Address> PatchAddress(string id, Delta<Address> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Address
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> PostAddress(Address item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAddress(string id)
        {
            return DeleteAsync(id);
        }
    }
}
