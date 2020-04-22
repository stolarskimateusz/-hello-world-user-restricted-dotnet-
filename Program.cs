using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;            // .Net 4.5 required for WebUtility.UrlEncode
using System.Drawing;        // Add reference
using Newtonsoft.Json;       // NuGet Json.NET
using Microsoft.AspNetCore.WebUtilities;
using System.Web;

namespace dn
{
    class Program
    {
        private static Random random = new Random();
        const string AuthorizationUri = "https://internal-dev.api.service.nhs.uk/oauth2/authorize";  // Authorization code endpoint
        const string RedirectUri = "https://nhsd-apim-testing-internal-dev.herokuapp.com/callback";  // Callback endpoint
        const string TokenUri = "https://internal-dev.api.service.nhs.uk/oauth2/token";  // Get tokens endpoint
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
       
        const string grant_type="authorization_code";
        const string client_id="Too5BdPayTQACdw1AJK1rD4nKUD0Ag7J";
        const string client_secret="wi7sCuFSgQg34ZWO";

        static void GetCode()
        {
            var _state = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            var dictionary = new Dictionary<string, string> {
                {"​client_id", client_id},
                {"​​redirect_uri", RedirectUri},
                {"response_type", "code"},
                {"state", _state},
            };
            string urls = QueryHelpers.AddQueryString(AuthorizationUri, dictionary);
            Console.WriteLine("Open link in your browser "+urls);

        }

        static void GetAccessToken()
        {
            Console.WriteLine("Enter callback url:");
            string CallbackUrl = Console.ReadLine();
            Uri siteUri = new Uri(CallbackUrl);
            string _code = HttpUtility.ParseQueryString(siteUri.Query).Get("code");
            string _state = HttpUtility.ParseQueryString(siteUri.Query).Get("state");

            var client = new RestClient(TokenUri);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter(
                "application/x-www-form-urlencoded",
                String.Format(
                    "grant_type=authorization_code&client_id={0}&client_secret={1}&redirect_uri={2}&code={3}",
                    client_id, client_secret, RedirectUri, _code
                ),
                ParameterType.RequestBody
            );
            IRestResponse response = client.Execute(request);
            var json = JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(json);

        }
        static void Main(string[] args)
        {
            GetCode();
            GetAccessToken();
        }
    }
}
