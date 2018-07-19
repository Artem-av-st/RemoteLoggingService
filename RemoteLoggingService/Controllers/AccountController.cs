using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;
using RemoteLoggingService.Services;
using RemoteLoggingService.ViewModels;

namespace RemoteLoggingService.Controllers
{

    public class AccountController : Controller
    {
        private AppDbContext db;
        public AccountController(AppDbContext context)
        {
            db = context;
        }
       
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);           
            return View();
        }

        
        [HttpPost]       
        public async Task<IActionResult> Login(LoginModel model)
        {            
            if(!Captcha.CheckCaptcha(Request))
            {
                ModelState.AddModelError("Captcha", "Captcha is invalid.");
                return View();
            }

            if (ModelState.IsValid)
            {        
                // Get user from DB
                User user = await db.Users.Include(x => x.UserRole).FirstOrDefaultAsync(u => u.Email == model.Email);

                // Check if credentials are correct
                if (user != null && Security.CheckPassword(model.Password, user.Password))
                {
                    // Check if user is approved
                    if (user.IsApproved)
                    {
                        // Check user role (Admin or Dev)
                        await Authenticate(model.Email, user.UserRole.Name=="Admin");
                        return RedirectToAction(nameof(MonitoringController.Index), "Monitoring");
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Your account is not approved yet. Contact your administrator.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Email", "Invalid email or/and Password");
                }
            }
            return View();
        }        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        
        [HttpPost]        
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!Captcha.CheckCaptcha(Request))
            {
                ModelState.AddModelError("Captcha", "Captcha is invalid.");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                // Check if user with the same email already exists in DB
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    var hashedPassword = Security.GetHashedPassword(model.Password);

                    // Get developer RoleId
                    var roleId = db.UserRoles.FirstOrDefault(x => x.Name == "Developer").Id;
                    // Add new user to DB
                    db.Users.Add(new User { Email = model.Email, Password = hashedPassword, UserId = Guid.NewGuid().ToString(), RoleId=roleId, Name = model.Name});
                    await db.SaveChangesAsync();
                   
                    return RedirectToAction(nameof(AccountController.Login), "Account");
                }
                else
                    ModelState.AddModelError("Email", "User with the same email already exists");
            }
            return View(model);
        }

        private async Task Authenticate(string userName, bool isAdmin)
        {            
            var claims = new List<Claim>
            {
                // User name claim
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
                
            };
            var role = String.Empty;
            if(isAdmin)
            {
                role = "Admin";
            }
            else
            {
                role = "Developer";
            }
            // User role claim
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            // Set cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new AuthenticationProperties() {ExpiresUtc=DateTimeOffset.UtcNow.AddHours(1) });
        }
       
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }        
    }
}