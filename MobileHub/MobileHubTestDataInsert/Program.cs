using System.Collections.Generic;
using System.Text;
using MobileHub.DAL;
using MobileHub.Models;

namespace MobileHubTestDataInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new MobileHubContext())
            {
                var users = new List<User>

            {
                new User
                {
                    FirstName = "Bledar", LastName = "Bitri",
                    Files = new List<UserFile>
                    {
                        new UserFile
                        {
                            Name = "TestFile 1",
                            Content = Encoding.Default.GetBytes("Here is some test text")
                        },
                        new UserFile
                        {
                            Name = "TestFile 2",
                            Content = Encoding.Default.GetBytes("The second file")
                        }
                    }
                },
                new User
                {
                    FirstName = "William", LastName = "Smith",
                    Files = new List<UserFile>
                    {
                        new UserFile
                        {
                            Name = "Smith 1",
                            Content = Encoding.Default.GetBytes("William Smiths first file")
                        },
                        new UserFile
                        {
                            Name = "W. Smith 2",
                            Content = Encoding.Default.GetBytes("William Smiths second file")
                        }
                    }
                }
            };
               users.ForEach(u => ctx.Users.Add(u));
               ctx.SaveChanges();
            }
        }
    }
}
