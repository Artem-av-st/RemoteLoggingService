using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace RemoteLoggingServiceTest
{

    public class AccountControllerTest
    {
        [Fact]
        public async void Try_to_authorize()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:61026")
            };
            var password = "14233241";
            var email = "artem-av-st@yandex.ru";
            var request = new HttpRequestMessage()
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "email", email }, { "password", password } }),
                Method = new HttpMethod("Post")
            };
            var result = await (await httpClient.SendAsync(request)).Content.ReadAsStringAsync();
            
            
            
        }
    }
}
