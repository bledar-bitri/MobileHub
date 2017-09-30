﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Contracts;
using Services;

namespace MobileHumWebApi.Controllers
{
    public class UserContractController : ApiController
    {
        public IEnumerable<UserContract> Get()
        {
            using (var service = new UserService())
            {
                return service.GetAllUsers();
            }
        }

        [Route("api/UserContract/{sectionType}/GetBySection")]
        public IEnumerable<UserContractForUserSelection> Get(string sectionType)
        {
            using (var service = new UserService())
            {
                return service.GetUsersForUserSelection();
            }
        }
        
        public UserContract Get(int id)
        {
            using (var service = new UserService())
            {
                return service.GetUser(id);
            }
        }

        [HttpPost]
        public HttpResponseMessage PostUsers(List<UserContract> users)
        {
            using (var service = new UserService())
            {
                string statistics;
                var newUsers = service.AddUsers(users, out statistics);

                var lang = Request.Headers.AcceptLanguage.FirstOrDefault();

                var response = Request.CreateResponse(HttpStatusCode.Created, users);
                response.Headers.Add("X-Status", statistics);
                response.Headers.Location = new Uri("http://localhost:8887/api/user/" + users[0].ID);

                return response;
            }
        }

        public void Delete(int id)
        {
        }
    }
}
