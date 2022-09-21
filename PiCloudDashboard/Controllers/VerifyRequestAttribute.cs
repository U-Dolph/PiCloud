using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using NuGet.Common;
using PiCloudDashboard.Data;
using PiCloudDashboard.Models;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PiCloudDashboard.Controllers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VerifyRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _jwt = filterContext.HttpContext.Session.GetString("JWT");

            var loginRoute = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Dashboard", action = "Login" }));

            //Check if JWT is set
            if (string.IsNullOrEmpty(_jwt))
            {
                filterContext.Result = loginRoute;
                return;
            }

            var _deserializedJWT = JsonConvert.DeserializeObject<JWT>(filterContext.HttpContext.Session.GetString("JWT"));

            HttpClient _client = new HttpClient();

            //Claim admin privilege
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _deserializedJWT.Token);

            //var webRequest = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7270/auth/validate-admin");

            var _response = _client.Send(new HttpRequestMessage(HttpMethod.Get, "https://localhost:7270/auth/validate-admin"));

            //Try to refresh the token
            if (_response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var _content = new StringContent(_jwt, Encoding.UTF8, "application/json");
                var _refreshResult = _client.Send(new HttpRequestMessage(HttpMethod.Post, "https://localhost:7270/auth/refreshToken")
                {
                    Content = new StringContent(_jwt, Encoding.UTF8, "application/json")
                });

                var _responseContent = _refreshResult.Content.ReadAsStringAsync().Result;
                var _responseJWT = JsonConvert.DeserializeObject<JWT>(_responseContent);

                if (_responseJWT.Result)
                {
                    filterContext.HttpContext.Session.SetString("JWT", JsonConvert.SerializeObject(new TokenDTO()
                    {
                        Token = _responseJWT.Token,
                        RefreshToken = _responseJWT.RefreshToken
                    }));

                    //Re-challenge for admin privileges
                    _client.DefaultRequestHeaders.Remove("Authorization");
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _responseJWT.Token);
                    _response = _client.Send(new HttpRequestMessage(HttpMethod.Get, "https://localhost:7270/auth/validate-admin"));
                }
            }

            if (!_response.IsSuccessStatusCode)
            {
                filterContext.Result = loginRoute;
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
