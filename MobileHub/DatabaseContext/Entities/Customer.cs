using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Entities
{
    public class Customer : Person
    {
        public string Title { get; set; }
        public string Duty { get; set; }
        public string Possition { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
