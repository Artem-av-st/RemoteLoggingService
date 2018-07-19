using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;
using RemoteLoggingService.Services;
using RemoteLoggingService.ViewModels;

namespace RemoteLoggingService.Controllers
{

    [Authorize]
    public class MonitoringController : Controller
    {
        private AppDbContext db;
        public MonitoringController(AppDbContext context)
        {
            db = context; 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get associated with logged user clients (for developer) or all clients (for Admin)
            var userClients = await UserServices.GetUserClients(db, User);

            // If there are no clients then redirect to "Create new client" page
            if (userClients.Count() == 0)
            {
                return RedirectToAction("Create", "Clients");
            }
            ViewBag.Clients = userClients;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShowLogs(GetLogsModel model)
        {
            var a = Request;
            ViewBag.ClientName = model.ClientName;
            // Check if dates are invalid
            if (model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "To Date cannot be less than From Date");
                ModelState.AddModelError("FromDate", "To Date cannot be less than From Date");
                ViewBag.Clients = await UserServices.GetUserClients(db, User);
                return View("Index");
            }

            // Get list of log messages (all if selected "Any")
            var logs = new List<Log>();
            if (model.LogType == "Any")
            {
                logs = await db.Logs.Include(x => x.User).Where(x =>
                    x.User.Name == model.ClientName &&
                    x.Time >= model.FromDate &&
                    x.Time <= model.ToDate &&
                    x.Message.CustomContains(model.FindText))
                    .OrderBy(x => x.Time)
                    .ToListAsync();
            }
            else
            {
                logs = await db.Logs.Include(x => x.User).Where(x =>
                    x.User.Name == model.ClientName &&
                    x.Time >= model.FromDate &&
                    x.Time <= model.ToDate &&
                    x.Status == model.LogType &&
                    x.Message.CustomContains(model.FindText))
                    .OrderBy(x => x.Time)
                    .ToListAsync();
            }

            return View(logs);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var log = await db.Logs
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ClientGuid == id);
            if (log == null)
            {
                return NotFound();
            }

            return View(log.User);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(User user)
        {
            // Delete all logs of client by id
            var logs = await db.Logs.Where(x => x.ClientGuid == user.UserId).ToListAsync();
            db.RemoveRange(logs);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> SaveToFile()
        {
            var model = GetExportParameters();
            var logs = new List<Log>();
            if (model.LogType == "Any")
            {
                logs = await db.Logs.Include(x => x.User).Where(x =>
                    x.User.Name == model.ClientName &&
                    x.Time >= model.FromDate &&
                    x.Time <= model.ToDate &&
                    x.Message.CustomContains(model.FindText))
                    .ToListAsync();
            }
            else
            {
                logs = await db.Logs.Include(x => x.User).Where(x =>
                    x.User.Name == model.ClientName &&
                    x.Time >= model.FromDate &&
                    x.Time <= model.ToDate &&
                    x.Status == model.LogType &&
                    x.Message.CustomContains(model.FindText))
                    .OrderBy(x => x.Time)
                    .ToListAsync();
            }            
            
            var stream = await GenerateCsvFromLogs(logs);
            var response = File(stream, "application/octet-stream", "Export.csv");
            return response;
        }

        private async Task<Stream> GenerateCsvFromLogs(List<Log> logs)
        {          
            // Header line
            var sb = new StringBuilder(",Date,Status,Message\n");

            // Replace characters which can corrupt CSV structure
            Regex rgx = new Regex("[\n,\"]");
            
            int index = 1;
            foreach(var log in logs)
            {
                sb.AppendFormat($"{index++},{log.Time},{log.Status.ToString()},{rgx.Replace(log.Message, "")}\n");
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(sb.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Returns entity of GetLogModel created from request string
        /// </summary>        
        private GetLogsModel GetExportParameters()
        {
            var parameters = new Dictionary<string, string>();            
            var requestString = Request.Headers["Referer"];
            Regex rgx = new Regex(@"(?<=[\?]).+$");

            // Get all text in request after '?' character
            var parametersString = rgx.Matches(requestString[0]);

            // Split parameters
            foreach (var value in parametersString[0].Value.Split("&"))
            {
                // Split parameter
                var element = value.Split("=");

                // Replace special characters and add to dictionary
                parameters.Add(element[0], element[1].Replace("+", " ").Replace("%3A", ":"));
            }

            return new GetLogsModel()
            {
                ClientName = parameters["ClientName"],
                FindText = parameters["FindText"],
                LogType = parameters["LogType"],
                FromDate = DateTime.Parse(parameters["FromDate"]),
                ToDate = DateTime.Parse(parameters["ToDate"]),
            };
        }
    }
}