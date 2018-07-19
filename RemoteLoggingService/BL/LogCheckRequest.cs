using RemoteLoggingService.BL.Inerfaces;
using RemoteLoggingService.Services.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace RemoteLoggingService.BL
{
    public class LogCheckRequest : ICheckRequest
    {
        public bool Check(HttpRequest request, out string response)
        {
            response = null;
            switch (request.Method)
            {
                case "GET": return CheckGetRequest(request, out response);
                case "POST": return CheckPostRequest(request, out response);
                default:
                    new JsonResponseBase(HttpStatusCode.InternalServerError, "Unknown HTTP method").ToString();
                    return false;
            }
        }

        private bool CheckPostRequest(HttpRequest request, out string response)
        {
            response = null;

            if (String.IsNullOrEmpty(request.Query["guid"]))
            {
                response = new JsonResponseBase(HttpStatusCode.BadRequest, "Request must has guid parameter.").ToString();
                return false;
            }
            var userRole = new SqlLogProvider().GetUserRole(request.Query["guid"]);
            if (userRole != "Admin" && userRole != "Client")
            {
                response = new JsonResponseBase(HttpStatusCode.Unauthorized, "You have no permission to this request").ToString();
                return false;
            }

            return true;
        }

        private bool CheckGetRequest(HttpRequest request, out string response)
        {
            response = null;
            if (String.IsNullOrEmpty(request.Query["guid"]))
            {
                response = new JsonResponseBase(HttpStatusCode.BadRequest, "Request must has guid parameter.").ToString();
                return false;
            }
            var userRole = new SqlLogProvider().GetUserRole(request.Query["guid"]);
            if (userRole != "Admin" && userRole != "Developer")
            {
                response = new JsonResponseBase(HttpStatusCode.Unauthorized, "You have no permission to this request").ToString();
                return false;
            }
            if (String.IsNullOrEmpty(request.Query["clientGuid"]))
            {
                response = new JsonResponseBase(HttpStatusCode.BadRequest, "Request must clientGuid parameter.").ToString();
                return false;
            }
            if (userRole == "Developer")
            {
                if(!new SqlLogProvider().CheckIfDeveloperAssignedToClient(request.Query["clientGuid"], request.Query["guid"]))
                {
                    response = new JsonResponseBase(HttpStatusCode.Unauthorized, "You have no permission to this request").ToString();
                    return false;
                }
                
            }
            
            return true;
        }
    }
}
