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
using RemoteLoggingService.Services;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace RemoteLoggingService.Controllers
{
    [Authorize]
    [Route("api")]
    public class LogController : Controller
    {
        private IRepository db;
        public LogController(IRepository repo)
        {
            db = repo;
        }

           
        [HttpGet("Log")]       
        public async Task<IActionResult> GetLogs(GetLogsModel model)
        {
            if(!ModelState.IsValid)
            {                
                return BadRequest(ModelState);
            }
            // Check if dates are invalid
            if (model.ToDate < model.FromDate)
            {
                ModelState.AddModelError("ToDate", "To Date cannot be less than From Date");
                ModelState.AddModelError("FromDate", "To Date cannot be less than From Date");
                return BadRequest(ModelState);
            }
                        
            var logs = await db.GetLogs(model);

            return new JsonResult(logs);
        }

        [Route("Log")]
        [HttpPost]              
        public async Task<IActionResult> CreateLogs([FromBody]IEnumerable<Log> logs)
        {            
            try
            {               
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!logs.Any())
                {
                    ModelState.AddModelError("Logs", "Cannot be empty");
                    return BadRequest(ModelState);
                }
                var clientGuid = logs.ElementAt(0).ClientId;
                if (!await db.ClientExists(clientGuid))
                {
                    ModelState.AddModelError("ClientGuid", "The GUID contained in the request must belong to one of the clients");
                    return BadRequest(ModelState);
                }
                if(!logs.All(x => x.ClientId == clientGuid))
                {
                    ModelState.AddModelError("ClientGuid", "All log messages MUST contain the same client ID as in request string GUID");
                    return BadRequest(ModelState);                    
                }

                // If message has fatal status then send notification to developer
                if(logs.Any(x => x.Status=="Fatal"))
                {
                    var sender = HttpContext.RequestServices.GetService<INotificationSender>();
                    await sender.Notify(logs.FirstOrDefault(x => x.Status == "Fatal"), db);
                }

                // Add messages to DB
                return new JsonResult(await db.AddRangeAndSave(logs)) {StatusCode = 201 };    
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpDelete("Log")]
        public async Task<IActionResult> Delete(Guid userId)
        {            
            var logs = await db.GetLogs(userId);
            await db.DeleteRangeAndSave(logs);

            return Ok();
        }

        [HttpGet("Log/ToFile")]        
        public async Task<IActionResult> SaveToFile(GetLogsModel model)
        {
            var logs = await db.GetLogs(model);

            var csvStream = GenerateCsvFromLogs(logs);
            
            return File(csvStream, "application/octet-stream", "Export.csv");
        }

        private Stream GenerateCsvFromLogs(IEnumerable<Log> logs)
        {
            // Header line
            var sb = new StringBuilder(",Date,Status,Message\n");

            // Replace characters which can corrupt CSV structure
            Regex rgx = new Regex("[\n,\"]");

            int index = 1;
            foreach (var log in logs)
            {
                sb.AppendFormat($"{index++},{log.Time},{log.Status.ToString()},{rgx.Replace(log.Message, "")}\n");
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
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