using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using LoginDatabaseContext;


namespace PopulateMobileHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Mobile Hub With Data");
            Console.WriteLine("*****************************************************\n\n");


            var securityUsers = SecurityDatabasePopulator.Populate();
            CustomerDatabasePopulator.Populate(securityUsers);
            
        }
    }
}
