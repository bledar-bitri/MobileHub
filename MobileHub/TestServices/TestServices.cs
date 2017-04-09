
using System;
using System.Reflection;
using Ninject;

namespace TestServices
{
    public class TestServices
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var test = new TestRouteService(kernel);
            test.TestRouteGenerationForUserId(1);

            Console.ReadLine();
        }
    }
}
