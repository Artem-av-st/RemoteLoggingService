using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemoteLoggingService.Services
{
    public static class UserServices
    {
        public static async Task<IEnumerable<Client>> GetUserClients(AppDbContext db, ClaimsPrincipal currentUser)
        {            
            var user = db.Users.Include(x => x.UserRole).FirstOrDefault(x => x.Email == currentUser.Identity.Name);
            var clients = new List<Client>();
            string role = currentUser.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "Admin")
            {
                clients = await db.Clients.Include(x => x.User).Include(x => x.Developer).ToListAsync();
            }
            else
            {
                clients = await db.Clients.Include(x => x.User).Include(x => x.Developer).Where(x => x.DeveloperId == user.UserId).ToListAsync();
            }
            return clients;
        }
        public static async Task<IEnumerable<User>> GetDevelopers(AppDbContext db, ClaimsPrincipal currentUser)
        {
            var user = db.Users.Include(x => x.UserRole).FirstOrDefault(x => x.Email == currentUser.Identity.Name);
            var developers = new List<User>();
            string role = currentUser.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if (role == "Admin")
            {
                return await db.Users.Include(x => x.UserRole).Where(x => x.UserRole.Name != "Client").ToListAsync();
            }
            else
            {
                return new List<User>() {user};
            }            
        }        
    }
}
