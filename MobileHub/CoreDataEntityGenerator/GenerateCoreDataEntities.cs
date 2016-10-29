using System;
using System.Collections.Generic;
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
                typeof(Item),
                typeof(Country),
                typeof(Address)
            };

            var sb = new StringBuilder();
            types.ForEach(t => sb.Append(Utilities.CoreDataEntityGenerator.GetCoreDataEntity(t)));

            Console.WriteLine(sb);

            Console.ReadLine();
        }
    }
}
