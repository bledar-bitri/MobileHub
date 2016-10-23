using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace TestWebApi
{
    public class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var client = new UserClient();
                var user = new UserContract()
                {
                    FirstName = "Test",
                    LastName = "Console",
                    DateOfBirth = new DateTime(1972, 11, 11),
                    EMailAddress = "testconsole@test.com"
                };

                var result = client.Post(new List<UserContract> {user});
//                var result = client.Post(null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void PrintWhite(string s)
        {
            Console.WriteLine(s, ConsoleColor.White);
        }
    }
}
