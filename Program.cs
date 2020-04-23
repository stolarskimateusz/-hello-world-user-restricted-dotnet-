using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;   
using Microsoft.AspNetCore.WebUtilities;
using System.Web;
using System.Net.Http;

namespace HelloWorldUserRestrictedAPI
{
    class OAuth
    {
        Random random = new Random();
        private string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private string AuthorizationUri = "https://internal-dev.api.service.nhs.uk/oauth2/authorize";  // Authorization code endpoint
        private string RedirectUri = "https://nhsd-apim-testing-internal-dev.herokuapp.com/callback";  // Callback endpoint
        private string TokenUri = "https://internal-dev.api.service.nhs.uk/oauth2/token";  // Get tokens endpoint       
        private string client_id="Too5BdPayTQACdw1AJK1rD4nKUD0Ag7J";  // change to your Client_ID
        private string client_secret="wi7sCuFSgQg34ZWO";  // change to your client_secret

        private string state;

        public void GetCode()
        {
            var _state = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            this.state = _state;
            var dictionary = new Dictionary<string, string>()
            {
                {"client_id", client_id},
                {"​​redirect_uri", RedirectUri},
                {"response_type", "code"},
                {"state", _state}
            };
            string url = QueryHelpers.AddQueryString(AuthorizationUri, dictionary);
            Console.WriteLine("Open link in your browser "+url);

        }

        public async void GetAccessToken()
        {
            Console.WriteLine("Enter callback url:");
            string CallbackUrl = Console.ReadLine();
            Uri siteUri = new Uri(CallbackUrl);
            string _code = HttpUtility.ParseQueryString(siteUri.Query).Get("code");
            string _state = HttpUtility.ParseQueryString(siteUri.Query).Get("state");

            if (this.state==_state) {

                HttpClient clienta = new HttpClient();
                clienta.DefaultRequestHeaders.Add("cache-control", "no-cache");
                clienta.DefaultRequestHeaders.Add("content-type", "application/x-www-form-urlencoded");
                var dict = new Dictionary<string, string>()
                {
                    {"grant_type", "authorization_code"},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                    {"redirect_uri", RedirectUri},
                    {"code", _code}
                };
                var req = new HttpRequestMessage(HttpMethod.Post, TokenUri) { Content = new FormUrlEncodedContent(dict) };
                var res = await clienta.SendAsync(req);
                res.EnsureSuccessStatusCode();

                var responseString = res.Content.ReadAsStringAsync().Result;

                var json = JsonConvert.DeserializeObject(responseString);
                Console.WriteLine(json);
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine("callback is malformed");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            OAuth OAuth = new OAuth();
            OAuth.GetCode();
            OAuth.GetAccessToken();
        }
    }
}
