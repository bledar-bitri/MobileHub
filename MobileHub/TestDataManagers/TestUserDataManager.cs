using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Managers.Security;

namespace TestDataManagers
{
    public class TestUserDataManager
    {
        const int userIdToRead = 17;
        public static void Test()
        {
            using (var mgr = new UserDataManager())
            {
                var user = mgr.GetUser(userIdToRead);
                if (user != null)
                {
                    Console.WriteLine("Got User: " + user.UserName);
                }
                else
                {
                    Console.WriteLine("Could not get user: " + userIdToRead);
                }
            }
        }
    }
}
