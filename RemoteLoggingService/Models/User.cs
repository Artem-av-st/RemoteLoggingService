using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteLoggingService.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }      
        public string Email { get; set; }       
        public string Password { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
                
        public UserRole UserRole { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
