using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// 
    /// </summary>
    public class CustomerController : TableController<Customer>
    {
        /// <inheritdoc />
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            var context = new MobileHubCustomerContext();
            DomainManager = new EntityDomainManager<Customer>(context, Request);
        }

        // GET: table/Customer
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<Customer> Get()
        {
            return Query();
        }

        // GET tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SingleResult<Customer> GetCustomer(string id)
        {
            return Lookup(id);
        }


        // PATCH tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patch"></param>
        /// <returns></returns>
        public async Task<Customer> PatchCustomer(string id, Delta<Customer> patch)
        {
            try
            {
                var current = await UpdateAsync(id, patch);
                return current;
            }
            catch
            {
                var current = await UpdateAsync(id, patch);
                return current;
            }
        }

        // POST tables/Customer
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> PostCustomer(Customer item)
        {
            var current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Customer/48D68C86-6EA6-4C25-AA33-223FC9A27959
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteCustomer(string id)
        {
            return DeleteAsync(id);
        }
    }
}
