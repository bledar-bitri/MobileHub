//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoginDatabaseContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class ActionHistory
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public int ActionCode { get; set; }
        public System.DateTime ActionTime { get; set; }
        public string Memo { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual CustomerUser CustomerUser { get; set; }
    }
}
