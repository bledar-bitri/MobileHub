using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace TestWebApi
{
    public class PostUserResult
    {
        public string Statistics { get; set; }
        public List<UserContract> Users { get; set; }
    }

}
