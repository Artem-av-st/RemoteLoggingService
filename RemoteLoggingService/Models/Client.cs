using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteLoggingService.Models
{

    public class Client
    {
        [Key]
        [ForeignKey("User")]
        public string ClientId { get; set; }

        [ForeignKey("Developer")]        
        public string DeveloperId { get; set; }

        public User User { get; set; }
        public User Developer { get; set; }

    }
}
