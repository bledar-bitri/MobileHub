using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPasswordService
    {
        string ComputeHash(string textPlain, string hashAlgorithm, byte[] salt);
        bool VerifyHash(string plainText, string hashAlgorithm, string hashValue);
    }
}
