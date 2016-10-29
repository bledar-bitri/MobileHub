using System;
using DataAccessLayer.Managers;

namespace TestDataManagers
{
    public class TestCustomerDataManager
    {
        const int UserIdToRead = 7;
        const int UserLocaleId = 7;
        public static void Test()
        {
            using (var mgr = new CustomerDataManager())
            {
                var customers = mgr.GetCustomerByAccountManager(UserIdToRead);
                if (customers != null)
                {
                    Console.WriteLine("Got {0} customers " , customers.Count);
                }
                else
                {
                    Console.WriteLine("Could not get any customers for user: " + UserIdToRead);
                }

                var history = mgr.GetCustomerActionsHistory(UserIdToRead, UserLocaleId.ToString());
                if (history != null)
                {
                    Console.WriteLine("Got {0} history ", history);
                }
                else
                {
                    Console.WriteLine("Could not get any history for user: " + UserIdToRead);
                }

            }
        }
    }
}
