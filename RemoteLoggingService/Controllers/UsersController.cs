using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;

namespace RemoteLoggingService.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext db;

        public UsersController(AppDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get list of all developers and admins
            var users = await db.Users.Include(u => u.UserRole).Where(x => x.UserRole.Name!="Client").ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users.SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await db.UserRoles.Where(x => x.Name!= "Client").ToListAsync();
            ViewBag.Roles = roles;
            return View(user);
        }
        
        [HttpPost]        
        public async Task<IActionResult> Edit(string id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }       
            
            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(user);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await db.Users.Include(u => u.UserRole).SingleOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        
        [HttpPost, ActionName("Delete")]        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await db.Users.SingleOrDefaultAsync(m => m.UserId == id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return db.Users.Any(e => e.UserId == id);
        }
    }
}
