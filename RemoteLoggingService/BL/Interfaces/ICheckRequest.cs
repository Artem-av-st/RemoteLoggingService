using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemoteLoggingService.Services.Model;
using Microsoft.AspNetCore.Http;

namespace RemoteLoggingService.BL.Inerfaces
{
    interface ICheckRequest
    {
         bool Check(HttpRequest request, out string response);
    }
}
