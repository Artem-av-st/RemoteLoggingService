using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RemoteLoggingService.Services.ViewModels;
using System.Data.SqlClient;
using System.IO;

namespace RemoteLoggingService.Models
{
    public class AppDbContext : DbContext
    {
        virtual public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Logs { get; set; }

        public AppDbContext()
        {   
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder
                .UseSqlServer(config.GetConnectionString("DefaultConnection"));
                //.UseLazyLoadingProxies();
        }

    }
}
