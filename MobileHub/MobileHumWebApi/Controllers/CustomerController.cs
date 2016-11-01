using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Contracts;
using CustomerModel;
using Microsoft.Azure.Mobile.Server;
using Services;


namespace MobileHumWebApi.Controllers
{
    public class CustomerController : TableController<Customer>
    {

        
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
            DomainManager = new EntityDomainManager<Customer>(context, Request);
        }

        // GET: table/Customer
        public IEnumerable<CustomerContract> Get()
        {
            using (var service = new CustomerService())
            {
                return service.GetAllCustomerContracts();
            }
        }

        // GET tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Customer GetCustomer(string id)
        {
            using (var service = new CustomerService())
            {
                return service.GetCustomer(id);
            }
        }


        // PATCH tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Customer> PatchCustomer(string id, Delta<Customer> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Customer
        public async Task<IHttpActionResult> PostCustomer(Customer item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCustomer(string id)
        {
            return DeleteAsync(id);
        }
    }
}
