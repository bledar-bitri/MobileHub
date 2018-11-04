using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Contracts;
using Services;

namespace MobileHumWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UserContractController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserContract> Get()
        {
            using (var service = new UserService())
            {
                return service.GetAllUsers();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionType"></param>
        /// <returns></returns>
        [Route("api/UserContract/{sectionType}/GetBySection")]
        public IEnumerable<UserContractForUserSelection> Get(string sectionType)
        {
            using (var service = new UserService())
            {
                return service.GetUsersForUserSelection();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserContract Get(int id)
        {
            using (var service = new UserService())
            {
                return service.GetUser(id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PostUsers(List<UserContract> users)
        {
            using (var service = new UserService())
            {
                var newUsers = service.AddUsers(users, out string statistics);

                var lang = Request.Headers.AcceptLanguage.FirstOrDefault();

                var response = Request.CreateResponse(HttpStatusCode.Created, users);
                response.Headers.Add("X-Status", statistics);
                response.Headers.Location = new Uri("http://localhost:8887/api/user/" + users[0].ID);

                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
        }
    }
}
