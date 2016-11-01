using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CustomerModel;

namespace CoreDataEntityGenerator
{
    public class GenerateCoreDataEntities
    {
        static void Main(string[] args)
        {


            var types = new List<Type>
            {
                typeof (ActionHistory),
                typeof (ActionType),
                typeof (Address),
                typeof (AvailableAction),
                typeof (Country),
                typeof (Customer),
                typeof (CustomerCompany),
                typeof (CustomerParentCompany),
                typeof (CustomerType),
                typeof (CustomerUser),
                typeof (Event),
                typeof (Item),
                typeof (Locale),
                typeof (Meeting),
                typeof (OrderHeader),
                typeof (OrderItem)
            };


            var sb = new StringBuilder();
            types.ForEach(t => sb.Append(Utilities.CoreDataEntityGenerator.GetCoreDataEntity(t)));
            int x = -900;
            int y = -900;

            types.ForEach(t =>
            {
                sb.Append(Utilities.CoreDataEntityGenerator.GetCoreDataEntityPosition(t, x, y));
                x -= 100;
                y -= 100;
            });


            Console.WriteLine(sb);

            Console.ReadLine();
        }
    }
}
