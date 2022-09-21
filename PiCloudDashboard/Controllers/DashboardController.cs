using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PiCloudDashboard.Data;
using PiCloudDashboard.Models;
using System.Net;
using System.Net.Http;
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

        [VerifyRequest]
        public IActionResult Index()
        {
            return View();
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("games")]
        [VerifyRequest]
        public async Task<IActionResult> Games()
        {
            var result = await getGamesList();

            if (result.Success)
            {
                List<Game> games = GameListConverter(result.Response);
                return View(games);
            }

            return Redirect("~/login");
        }

        [Route("game/{id}")]
        [VerifyRequest]
        public async Task<IActionResult> Game(int id)
        {
            var result = await getGameById(id);

            if (result.Success)
            {
                Game game = JsonConvert.DeserializeObject<Game>(result.Response);
                return View(game);
            }

            return Redirect("~/login");
        }

        [Route("edit/{id}")]
        [VerifyRequest]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await getGameById(id);

            if (result.Success)
            {
                Game game = JsonConvert.DeserializeObject<Game>(result.Response);
                return View(game);
            }

            return Redirect("~/login");
        }


        #region background
        public async Task<IActionResult> LoginUser(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7270/auth/login", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var jwt = JsonConvert.DeserializeObject<JWT>(responseContent);

                        if (jwt.Result)
                        {
                            HttpContext.Session.SetString("JWT", JsonConvert.SerializeObject(new TokenDTO() { Token = jwt.Token, RefreshToken = jwt.RefreshToken }));

                            //Validate admin login
                            var authResponse = await ValidateAdminLogin(jwt);

                            if (!authResponse.Success)
                                return Redirect("~/login");

                            return Redirect("/");
                        }
                    }

                    return Redirect("~/login");
                }
            }
        }

        public IActionResult LogoutUser()
        {
            HttpContext.Session.Remove("JWT");
            return Redirect("~/login");
        }

        public async Task<ResponseResult> ValidateAdminLogin(JWT token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                var response = await httpClient.GetAsync("https://localhost:7270/auth/validate-admin");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new ResponseResult()
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Response = "Invalid privileges"
                    };

                else if (!response.IsSuccessStatusCode)
                    return new ResponseResult()
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Response = $"Unknown error [{response.Content}]"
                    };

                else
                    return new ResponseResult()
                    {
                        Success = true,
                        StatusCode = HttpStatusCode.OK
                    };
            }
        }



        private async Task<ResponseResult> getGamesList()
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                using (var response = await httpClient.GetAsync("https://localhost:7270/admin-api/games/"))
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

        private async Task<ResponseResult> getGameById(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                using (var response = await httpClient.GetAsync($"https://localhost:7270/admin-api/game/{id}"))
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

        [VerifyRequest]
        public async Task<IActionResult> EditGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return Redirect($"~/edit/{game.Id}");
            }

            game.LastUpdated = DateTime.Now;

            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PatchAsync($"https://localhost:7270/admin-api/update/{game.Id}", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return Redirect("~/games");
                    }

                    return Redirect($"~/edit/{game.Id}");
                }
            }
        }
        #endregion
    }
}
