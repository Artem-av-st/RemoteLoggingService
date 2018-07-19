using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteLoggingService.Models
{
    public class User
    {
        public string UserId { get; set; }      
        public string Email { get; set; }       
        public string Password { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }

        [ForeignKey("UserRole")]
        public int RoleId { get; set; }
        public virtual UserRole UserRole { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
