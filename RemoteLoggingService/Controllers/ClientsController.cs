using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RemoteLoggingService.Services.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemoteLoggingService.Models;
using RemoteLoggingService.Services;

namespace RemoteLoggingService.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly AppDbContext db;

        public ClientsController(AppDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get associated with logged user clients (fro developer) or all clients (for Admin)
            var clients = await UserServices.GetUserClients(db, User);
            
            return View(clients);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Get list of all developers (for Admin role) or just logged developer for developer role
            var developers = await UserServices.GetDevelopers(db, User);
            ViewBag.Developers = developers;
            
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string clientName, string developerGuid)
        {
            // Check client name
            if(String.IsNullOrEmpty(clientName))
            {
                ModelState.AddModelError("Developer", "Client name cannot be empty");
                var developers = await UserServices.GetDevelopers(db, User);
                ViewBag.Developers = developers;
                return View();
            }

            // Create unique GUID
            var newUserGuid = String.Empty;
            do
            {
                newUserGuid = Guid.NewGuid().ToString();
            }
            while (await db.Users.FirstOrDefaultAsync(x => x.UserId == newUserGuid) != null);

            AddNewClientToDb(newUserGuid, clientName, developerGuid);
            
            return RedirectToAction(nameof(Index));
           
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get list of all developers (for Admin role) or just logged developer for developer role
            var developers = await UserServices.GetDevelopers(db, User);
            ViewBag.Developers = developers;

            // Get client by id
            var client = await db.Clients.Include(x => x.User).SingleOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }
            
            return View(client);
        }
       
        [HttpPost]        
        public async Task<IActionResult> Edit(string id, Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(client);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(client);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await db.Clients
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }
        
        [HttpPost, ActionName("Delete")]        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Get Entity from users and clients tables
            var client = await db.Clients.SingleOrDefaultAsync(m => m.ClientId == id);
            var user = await db.Users.SingleOrDefaultAsync(x => x.UserId == id);

            // Delete client first
            db.Clients.Remove(client);
            await db.SaveChangesAsync();

            // Delete user
            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return db.Clients.Any(e => e.ClientId == id);
        }

        private void AddNewClientToDb(string clientGuid, string clientName, string developerGuid)
        {
            // Get connection string
            var settingsText = System.IO.File.ReadAllText(@"appsettings.json");
            var connectionString = JsonConvert.DeserializeObject<AppSettings>(settingsText).ConnectionStrings.DefaultConnection;            

            // Execute stored procedure
            using (var connection = new SqlConnection(connectionString))
            {
                // Stored procedure name
                var command = new SqlCommand("AddNewClient", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClientGuid", clientGuid);
                command.Parameters.AddWithValue("@DeveloperGuid", developerGuid);
                command.Parameters.AddWithValue("@ClientName", clientName);
                command.Parameters.Add("@Result", System.Data.SqlDbType.NVarChar, 100).Direction = System.Data.ParameterDirection.Output;
                connection.Open();
                command.ExecuteNonQuery();

                // Check result
                if(!String.IsNullOrEmpty(command.Parameters["@Result"].Value.ToString()))
                {
                    throw new Exception("Add new user exception: " + command.Parameters["@Result"].Value.ToString());
                }

            }
        }

    }
}
