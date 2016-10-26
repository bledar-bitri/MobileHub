using System;
using System.Collections.Generic;
using LoginDatabaseContext;
using Services;
using Services.Interfaces;

namespace PopulateMobileHub
{
    public class SecurityDatabasePopulator
    {
        private static IPasswordService service = new PasswordService();
        private static string bradsPassword = "@#$SsomehiddenText*&^";
        private static string passwordHashAlgorithm = "SHA512";

        public static  List<User> Populate()
        {
            Console.WriteLine("*****************************************************");
            Console.WriteLine("Populating Security Database With Data");
            Console.WriteLine("*****************************************************\n\n");
            
            using (var ctx = new MobileHubSecurityContext())
            {

                #region Users

                var brad = new User
                {
                    FirstName = "Brad",
                    LastName = "Pit",
                    UserName = "b.pit",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "bledi1@yahoo.com",
                    LastLogonTime = DateTime.Now,

                };
                var george = new User
                {
                    FirstName = "George",
                    LastName = "Clooney",
                    UserName = "g.clooney",
                    DateOfBirth = new DateTime(1977, 10, 13),
                    EMailAddress = "bledi1@yahoo.com",
                    LastLogonTime = DateTime.Now,
                };

                var users = new List<User> {brad, george};

                #endregion

                #region Logins

                var bradsLogin = new Login
                {
                    UserName = "brad.bit",
                    PasswordHash = service.ComputeHash(bradsPassword, passwordHashAlgorithm, null),
                    User = brad
                };

                var logins = new List<Login>
                {
                    bradsLogin
                };

                #endregion


                #region Roles
                var roleUser = new Role
                {
                    Name = "User"
                };
                var roleAdmin = new Role
                {
                    Name = "Admin"
                };
                var roles = new List<Role> {roleUser, roleAdmin };
                #endregion


                #region Accounts
                var accountUser = new Account
                {
                    Name = "User",
                    PlanLevel = "Monthly"
                };
                var accountAdmin = new Account
                {
                    Name = "Admin",
                    PlanLevel = "Unlimited"
                };
                var accounts = new List<Account> {accountUser, accountAdmin};
                #endregion


                #region User Companies
                var companyEcp = new UserCompany
                {
                    Name = "ECP",
                    AccessLevel = "Full",
                    Account = accountUser
                };
                var companyHtb = new UserCompany
                {
                    Name = "HTB",
                    AccessLevel = "Full",
                    Account = accountAdmin
                };
                var userCompanies = new List<UserCompany> { companyEcp, companyHtb};
                #endregion


                #region Memberships
                var bradsMembership1 = new Membership
                {
                    User = brad,
                    UserCompany = companyEcp,
                    Role = roleUser,
                    AccountEmailAddress = "brad@email.com"  
                };
                var bradsMembership2 = new Membership
                {
                    User = brad,
                    UserCompany = companyEcp,
                    Role = roleAdmin,
                    AccountEmailAddress = "brad@email.com"
                };

                var memberShips = new List<Membership> { bradsMembership1, bradsMembership2 };
                #endregion

                Console.WriteLine("Adding data to the context...\n");
                users.ForEach(u=> ctx.Users.Add(u));
                logins.ForEach(l => ctx.Logins.Add(l));
                roles.ForEach(r => ctx.Roles.Add(r));
                accounts.ForEach(a => ctx.Accounts.Add(a));
                userCompanies.ForEach(c => ctx.UserCompanies.Add(c));
                memberShips.ForEach(m=>ctx.Memberships.Add(m));
                Console.WriteLine("\nSaving Changes ...\n");
                ctx.SaveChanges();
                Console.WriteLine("\nDONE!");

                return users;
            }
        }
    }
}
