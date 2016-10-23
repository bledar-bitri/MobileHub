using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Entities
{
    public class Meeting : Entity
    {
        public DateTime MeetingTime { get; set; }

        public string Purpose { get; set; }

        public string Memo { get; set; }

        public int AddressID { get; set; }
        public virtual Address Address { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }

        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

    }
}
