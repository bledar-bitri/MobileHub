using System;
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
