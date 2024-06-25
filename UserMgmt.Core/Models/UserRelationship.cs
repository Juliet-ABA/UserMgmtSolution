using System;
using System.Collections.Generic;
using System.Text;

namespace UserMgmt.Core.Models
{
    public class UserRelationship
    {
        public int UserRelationshipId { get; set; } // Auto-incrementing primary key
        public int ClientId { get; set; } // Foreign key referencing the User (client)
        public int ManagerId { get; set; } // Foreign key referencing the User (manager)
      
        public virtual User Client { get; set; }
        public virtual User Manager { get; set; }
    }

}
