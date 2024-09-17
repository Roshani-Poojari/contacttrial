using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactAppRoleBasedLogin.Models
{
    public class Contact
    {
        public virtual Guid Id { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual User User { get; set; }
        public virtual IList<ContactDetail> ContactDetails { get; set; } = new List<ContactDetail>(); //nullable

    }
}