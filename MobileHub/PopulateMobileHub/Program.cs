using System;


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
