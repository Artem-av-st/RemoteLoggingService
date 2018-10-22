using System.Collections.Generic;

namespace RemoteLoggingService.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }

        public UserRole()
        {
            Users = new List<User>();
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
