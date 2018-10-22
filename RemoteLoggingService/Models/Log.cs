﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteLoggingService.Models
{
    public class Log
    {
        public Log() { }
        public Log(Guid clientId, DateTime time, string message, string status, string metadata)
        {
            ClientId = clientId;
            Time = time;
            Message = message;
            Metadata = metadata;
            Status = status;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        [RegularExpression(@"[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}")]
        public Guid ClientId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Message { get; set; }
        
        public string Metadata { get; set; }       

        public User User { get; set; }
    }
}
