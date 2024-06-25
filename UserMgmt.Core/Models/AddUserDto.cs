using System;
using System.Collections.Generic;
using System.Text;

namespace UserMgmt.Core.Models
{
  public class AddUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public string Position { get; set; } 
        public int Level { get; set; }    

    }
}
