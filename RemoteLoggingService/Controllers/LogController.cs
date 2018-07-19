using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RemoteLoggingService.ViewModels;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RemoteLoggingService.Models;
using System.Threading.Tasks;
using RemoteLoggingService.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace RemoteLoggingService.Controllers
{

    public class LogController : Controller
    {
        private AppDbContext db;
        public LogController(AppDbContext context)
        {
            db = context;
        }

        [Authorize]
        [Route("api/[controller]")]
        [HttpGet]
        public IActionResult Get([FromQuery]LogModel model)
        {            
            try
            {
                // Check parameters
                if (!ModelState.IsValid)
                {
                    return new JsonResult(new JsonResponseBase(HttpStatusCode.BadRequest, "Request is invalid. Please check your request"));
                }
                if (!db.Clients.Any(x => x.ClientId == model.Guid.ToString()))
                {
                    return new JsonResult(new JsonResponseBase(HttpStatusCode.BadRequest, "The GUID contained in the request must belong to one of the clients"));
                }

                // Get logs from DB               
                var logs = db.Logs.Where(x => x.ClientGuid == model.Guid.ToString());                

                return new JsonResult(new JsonResponseLogs(HttpStatusCode.OK, logs));
            }
            catch(Exception e)
            {
                return new JsonResult(new JsonResponseBase(HttpStatusCode.InternalServerError, e.Message));
            }
        }
        
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery]LogModel model, [FromBody]IEnumerable<Log> logs)
        {            
            try
            {
                // Check parameters
                if(!ModelState.IsValid)
                {
                    return new JsonResult(new JsonResponseBase(HttpStatusCode.BadRequest, "Request is invalid. Please check your request"));
                }
                if(!db.Clients.Any(x => x.ClientId == model.Guid.ToString()))
                {
                    return new JsonResult(new JsonResponseBase(HttpStatusCode.BadRequest, "The GUID contained in the request must belong to one of the clients"));
                }
                if(!logs.All(x => x.ClientGuid == model.Guid.ToString()))
                {
                    return new JsonResult(new JsonResponseBase(HttpStatusCode.BadRequest, "All log messages MUST contain the same client ID as in request string GUID"));
                }

                // If message has fatal status then send notification to developer
                if(logs.Any(x => x.Status=="Fatal"))
                {
                    var sender = HttpContext.RequestServices.GetService<INotificationSender>();
                    sender.Notify(logs.FirstOrDefault(x => x.Status == "Fatal"), db);
                }

                // Add messages to DB
                db.Logs.AddRange(logs);
                await db.SaveChangesAsync();                
                
                return new JsonResult(new JsonResponseBase(HttpStatusCode.OK, "Successfully logged"));    
            }
            catch (Exception e)
            {
                return new JsonResult(new JsonResponseBase(HttpStatusCode.InternalServerError, e.Message));
            }
        }
    }
}