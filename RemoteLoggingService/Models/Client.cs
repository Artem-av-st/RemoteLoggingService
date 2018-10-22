using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteLoggingService.Models
{

    public class Client:User
    {        
        public User Developer { get; set; }
    }
}
