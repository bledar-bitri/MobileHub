using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestWebApi
{
    public class HttpClientHelper
    {
        public static string GetBaseUrl()
        {
            return "http://localhost:55944/api";
        }

        public static HttpClient CreateClient()
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Accept.Add(
            //       new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset = utf - 8");

            return client;
        }
    }

}
