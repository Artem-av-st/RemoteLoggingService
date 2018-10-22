using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;

namespace RemoteLoggingService.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly IRepository db;
        
        public UsersController(IRepository repo)
        {        
            db = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {            
            // Get list of all developers and admins
            var users = (await db.GetAllUsers()).Where(x => x.UserRole.Name!="Client").ToList();            
            return new JsonResult(users);           
        }
        
        [HttpPost]        
        public async Task<IActionResult> Edit(User user)
        {   
            if (ModelState.IsValid)
            {
                try
                {
                    await db.UpdateAndSave(user);                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await db.UserExists(user.Id))
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
            
            return NotFound();
        }
        
        [HttpDelete]        
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = db.GetUserById(id);
            if(user == null)
                return StatusCode(204);
            await db.DeleteAndSave(user);
            return StatusCode(200);
        }       
    }
}
