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
using RemoteLoggingService.ViewModels;

namespace RemoteLoggingService.Controllers
{
    [Authorize]
    [Route("api")]
    public class ClientsController : Controller
    {        
        private readonly IRepository db;

        public ClientsController(IRepository repo)
        {
            db = repo;
        }

        [HttpGet("Clients")]
        public async Task<IActionResult> GetClients()
        {
            // Get associated with logged user clients (for developer) or all clients (for Admin)
            var loggedInUser = await db.GetUserByEmail(User.Identity.Name);
            var clients = await db.GetUserClients(loggedInUser.Id);

            return new JsonResult(clients);
        }

        [HttpGet("Clients/{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await db.GetClientById(id);
           

            if (client.Developer.Email == User.Identity.Name)
            {
                client.UserRole.Users = null;
                client.Developer.UserRole.Users = null;
                return new JsonResult(client, new JsonSerializerSettings() { });
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [HttpPost("Clients")]        
        public async Task<IActionResult> CreateClient([FromBody]ClientCreateModel model)
        {
            // Check client name
            if(String.IsNullOrEmpty(model.ClientName))
            {
                ModelState.AddModelError("ClientName", "Client name cannot be empty");                
                return BadRequest(ModelState);
            }
            if((await db.GetAllClients()).FirstOrDefault(c => c.Name == model.ClientName) != null)
            {
                ModelState.AddModelError("ClientName", "Client with the same name already exists");
                return BadRequest(ModelState);
            }
            var currentUser = await db.GetUserByEmail(User.Identity.Name);
            
            var newClient = new Client()
            {
                Name = model.ClientName,
                UserRole = await db.GetUserRoleByName("Client"),
                Developer = currentUser
            };
          
            newClient = await db.AddAndSave(newClient);
                        
            return Created(Url.Content($"~/api/Clients/{newClient.Id}"), newClient);
           
        }      
       
        [HttpPut("Clients")]        
        public async Task<IActionResult> Edit([FromBody]Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await db.UpdateAndSave(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await db.ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }      
        
        [HttpDelete("Clients/{id}")]        
        public async Task<IActionResult> Delete(Guid id)
        {
            // Get Entity from users and clients tables
            var client = await db.GetClientById(id);
            if (client == null)
            {
                return NotFound();
            }

            // Delete user
            await db.DeleteAndSave(client);            

            return Ok();
        }
    }
}
