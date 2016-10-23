using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using Services.Interfaces;

namespace TestServices
{
    public class TestPasswordService
    {

        static void Main(string[] args)
        {
            IPasswordService service = new PasswordService();
            string password = "myP@5sw0rd";  // original password
            string passwordHash1 = service.ComputeHash(password, "SHA512", null);
            Console.WriteLine("SHA512: {0}", passwordHash1);
            string passwordHash2 = service.ComputeHash(password, "SHA512", null);
            Console.WriteLine("SHA512: {0}", passwordHash2);
            string passwordHash3 = service.ComputeHash(password, "SHA512", null);
            Console.WriteLine("SHA512: {0}", passwordHash3);
            string passwordHash4 = service.ComputeHash(password, "SHA512", null);
            Console.WriteLine("SHA512: {0}", passwordHash4);

            Console.WriteLine("Is Valid: {0}", service.VerifyHash(password, "SHA512", passwordHash1));
            Console.WriteLine("Is Valid: {0}", service.VerifyHash(password, "SHA512", passwordHash2));
            Console.WriteLine("Is Valid: {0}", service.VerifyHash(password, "SHA512", passwordHash3));
            Console.WriteLine("Is Valid: {0}", service.VerifyHash(password, "SHA512", passwordHash4));


            Console.ReadLine();



        }

        static void Main2(string[] args)
        {
            IPasswordService service = new PasswordService();

            string password = "myP@5sw0rd";  // original password
            string wrongPassword = "password";    // wrong password

            string passwordHashMD5 = service.ComputeHash(password, "MD5", null);
            string passwordHashSha1 = service.ComputeHash(password, "SHA1", null);
            string passwordHashSha256 = service.ComputeHash(password, "SHA256", null);
            string passwordHashSha384 = service.ComputeHash(password, "SHA384", null);
            string passwordHashSha512 = service.ComputeHash(password, "SHA512", null);

            Console.WriteLine("COMPUTING HASH VALUES\r\n");
            Console.WriteLine("MD5   : {0}", passwordHashMD5);
            Console.WriteLine("SHA1  : {0}", passwordHashSha1);
            Console.WriteLine("SHA256: {0}", passwordHashSha256);
            Console.WriteLine("SHA384: {0}", passwordHashSha384);
            Console.WriteLine("SHA512: {0}", passwordHashSha512);
            Console.WriteLine("");

            Console.WriteLine("COMPARING PASSWORD HASHES\r\n");
            Console.WriteLine("MD5    (good): {0}",
                                service.VerifyHash(
                                password, "MD5",
                                passwordHashMD5).ToString());
            Console.WriteLine("MD5    (bad) : {0}",
                                service.VerifyHash(
                                wrongPassword, "MD5",
                                passwordHashMD5).ToString());
            Console.WriteLine("SHA1   (good): {0}",
                                service.VerifyHash(
                                password, "SHA1",
                                passwordHashSha1).ToString());
            Console.WriteLine("SHA1   (bad) : {0}",
                                service.VerifyHash(
                                wrongPassword, "SHA1",
                                passwordHashSha1).ToString());
            Console.WriteLine("SHA256 (good): {0}",
                                service.VerifyHash(
                                password, "SHA256",
                                passwordHashSha256).ToString());
            Console.WriteLine("SHA256 (bad) : {0}",
                                service.VerifyHash(
                                wrongPassword, "SHA256",
                                passwordHashSha256).ToString());
            Console.WriteLine("SHA384 (good): {0}",
                                service.VerifyHash(
                                password, "SHA384",
                                passwordHashSha384).ToString());
            Console.WriteLine("SHA384 (bad) : {0}",
                                service.VerifyHash(
                                wrongPassword, "SHA384",
                                passwordHashSha384).ToString());
            Console.WriteLine("SHA512 (good): {0}",
                                service.VerifyHash(
                                password, "SHA512",
                                passwordHashSha512).ToString());
            Console.WriteLine("SHA512 (bad) : {0}",
                                service.VerifyHash(
                                wrongPassword, "SHA512",
                                passwordHashSha512).ToString());

            Console.ReadLine();
        }
    }
}
