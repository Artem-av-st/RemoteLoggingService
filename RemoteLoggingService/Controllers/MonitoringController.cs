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
        private IRepository db;
        public MonitoringController(IRepository repo)
        {
            db = repo;
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
                
                return View("Index");
            }

            // Get list of log messages (all if selected "Any")
            var logs = await db.GetLogs(model);
               
            return View(logs);
        }       

        
    }
}