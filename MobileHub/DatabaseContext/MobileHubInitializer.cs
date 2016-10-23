using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace DatabaseContext
{
    public class MobileHubInitializer : DropCreateDatabaseIfModelChanges<MobileHubContext>
    {
        protected override void Seed(MobileHubContext context)
        {
            /*
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
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();
            */
        }
    }
}
