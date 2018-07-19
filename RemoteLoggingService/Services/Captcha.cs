using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RemoteLoggingService.Services
{
    internal class Captcha
    {

        public static bool CheckCaptcha(HttpRequest request)
        {            
            var captcha = request.Form["g-recaptcha-response"];
            if(String.IsNullOrEmpty(captcha))
            {
                return false;
            }
            var secret = "6LdLcFQUAAAAAA6SrRZTlGSSf9rXIarNKz-esqJK";
            var client = new System.Net.WebClient();
            var googleReply = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={captcha}");

            Boolean.TryParse(JsonConvert.DeserializeObject<Captcha>(googleReply).Success, out var captchaResponse);
            return captchaResponse;
        }

        [JsonProperty("success")]
        private string Success { get; set; }        
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }   
    }
}
