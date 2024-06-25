using System.Collections.Generic;

namespace UserMgmt.Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }

       
        public virtual ICollection<UserRelationship> ClientRelationships { get; set; }

        public virtual ICollection<UserRelationship> ManagerRelationships { get; set; }

    }

}
