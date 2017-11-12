using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Mobile.Server;
using CustomerModel;
using Services;

namespace MobileHubWebApiCore.Controllers
{
    [Produces("application/json")]
    [Microsoft.AspNetCore.Mvc.Route("tables/Address")]
    public class AddressController : TableController<Address>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
            DomainManager = new EntityDomainManager<Address>(context, Request);
        }

        // GET: table/User
        public IEnumerable<Address> Get()
        {
            using (var service = new AddressService())
            {
                return service.GetAllAddresses();
            }
        }

        // GET tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Address GetAddress(string id)
        {
            using (var service = new AddressService())
            {
                return service.GetAddress(id);
            }
        }


        // PATCH tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Address> PatchAddress(string id, Delta<Address> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Address
        public async Task<IHttpActionResult> PostAddress(Address item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Address/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAddress(string id)
        {
            return DeleteAsync(id);
        }
    }
}