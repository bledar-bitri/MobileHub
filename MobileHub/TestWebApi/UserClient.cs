using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace TestWebApi
{

    public class UserClient
    {
        public async Task<UserContract> Get(int pNummer)
        {
            var client = HttpClientHelper.CreateClient();

            var url = $"{HttpClientHelper.GetBaseUrl()}/User/{pNummer}";

            var response = await client.GetAsync(url);
            return await response.Content.ReadAsAsync<UserContract>();

        }

        public async Task<List<UserContract>> Get(string name)
        {
            var client = HttpClientHelper.CreateClient();

            var url = $"{HttpClientHelper.GetBaseUrl()}/User?name={Uri.EscapeUriString(name)}";

            var response = await client.GetAsync(url);
            return await response.Content.ReadAsAsync<List<UserContract>>();

        }



        public async Task<PostUserResult> Post(List<UserContract> users)
        {
            var client = HttpClientHelper.CreateClient();

            var url = $"{HttpClientHelper.GetBaseUrl()}/User";

            
            var response = client.PostAsJsonAsync(url, users).Result;

            var saved = await response.Content.ReadAsAsync<List<UserContract>>();
            var statistik = response.Headers.GetValues("X-Statistik").FirstOrDefault();

            var result = new PostUserResult
            {
                Users = saved,
                Statistics = statistik
            };
            return result;
        }
    }
}

