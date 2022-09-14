using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PiCloudDashboard.Data;
using PiCloudDashboard.Models;
using System.Net;
using System.Text;

namespace PiCloudDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return Redirect("~/login");

            return View();
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Games()
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return Redirect("~/login");


            var result = await getGamesList();

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshResult = await refreshJwt(HttpContext.Session.GetString("JWT"));

                if (!refreshResult.Success)
                    return Redirect("~/login");

                _logger.Log(LogLevel.Information, "refresh success");
                
                result = await getGamesList();
            }

            if (result.Success)
            {
                List<Game> games = GameListConverter(result.Response);
                return View(games);
            }

            return Redirect("~/login");
        }

        public async Task<IActionResult> LoginUser(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7270/api/Authentication/login", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var jwt = JsonConvert.DeserializeObject<JWT>(responseContent);

                        if (jwt.Result)
                        {
                            HttpContext.Session.SetString("JWT", JsonConvert.SerializeObject(new TokenDTO() { Token = jwt.Token, RefreshToken = jwt.RefreshToken}));
                            return Redirect("/");
                        }
                    }

                    return Redirect("~/login");
                }
            }
        }

        private async Task<ResponseResult> getGamesList()
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                using (var response = await httpClient.GetAsync("https://localhost:7270/api/Games/"))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    return new ResponseResult()
                    {
                        Success = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode,
                        Response = responseContent
                    };
                }
            }
        }

        private List<Game> GameListConverter(string response)
        {
            return JsonConvert.DeserializeObject<List<Game>>(response);
        }

        private async Task<ResponseResult> refreshJwt(string tokenToRefresh)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(tokenToRefresh, Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7270/api/Authentication/refreshToken", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var jwt = JsonConvert.DeserializeObject<JWT>(responseContent);

                    if (jwt.Result)
                        HttpContext.Session.SetString("JWT", JsonConvert.SerializeObject(new TokenDTO() { Token = jwt.Token, RefreshToken = jwt.RefreshToken }));
                    
                    return new ResponseResult()
                    {
                        Success = jwt.Result,
                        StatusCode = response.StatusCode,
                        Response = responseContent
                    };
                }
            }
        }
    }
}
