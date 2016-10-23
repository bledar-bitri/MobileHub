using System;

namespace DatabaseContext.Entities
{
    public class User : Person
    {
        public string UserName { get; set; }
        
        public DateTime? LastLogonTime { get; set; }

    }
}
