using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Entities
{
    public class Country : Entity
    {
        public string Abbreviation { get; set; }
        public string Name { get; set; }
        
    }
}
