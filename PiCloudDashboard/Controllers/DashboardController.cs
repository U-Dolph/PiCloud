using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PiCloudDashboard.Data;
using PiCloudDashboard.Models;
using PiCloudDashboard.Services;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace PiCloudDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IFileUploadService _uploadService;
        private readonly IConfiguration _configuration;

        private string ApiUrl;

        public DashboardController(ILogger<DashboardController> logger, IFileUploadService fileUploadService, IConfiguration configuration)
        {
            _logger = logger;
            _uploadService = fileUploadService;
            _configuration = configuration;

            ApiUrl = _configuration.GetSection("APIUrl").Value;
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



        #region Game routes
        [Route("games")]
        [VerifyRequest]
        public async Task<IActionResult> Games()
        {
            var result = await getGamesList();

            if (result.Success)
            {
                List<Game> games = GameListConverter(result.Response);
                return View("~/Views/Dashboard/Games/Games.cshtml", games);
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
                return View("~/Views/Dashboard/Games/Game.cshtml", game);
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
                return View("~/Views/Dashboard/Games/Edit.cshtml", game);
            }

            return Redirect("~/login");
        }

        [Route("add")]
        [VerifyRequest]
        public async Task<IActionResult> Add()
        {
            return View("~/Views/Dashboard/Games/Add.cshtml");
        }
        #endregion


        #region User routes
        [Route("users")]
        [VerifyRequest]
        public async Task<IActionResult> Users()
        {
            var result = await getUsersList();

            if (result.Success)
            {
                List<User> users = UserListConverter(result.Response);
                return View("~/Views/Dashboard/Users/Users.cshtml", users);
            }

            return Redirect("~/login");
        }

        [Route("user/{id}")]
        [VerifyRequest]
        public async Task<IActionResult> User(string id)
        {
            var result = await getUserById(id);
            var roleResult = await getUsersRoles(id);

            if (result.Success && roleResult.Success)
            {
                User user = JsonConvert.DeserializeObject<User>(result.Response);
                user.Roles = JsonConvert.DeserializeObject<List<string>>(roleResult.Response);

                return View("~/Views/Dashboard/Users/User.cshtml", user);
            }

            return Redirect("~/login");
        }
        #endregion


        #region background
        public async Task<IActionResult> LoginUser(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"{ApiUrl}/auth/login", content))
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

                var response = await httpClient.GetAsync($"{ApiUrl}/auth/validate-admin");

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

                using (var response = await httpClient.GetAsync($"{ApiUrl}/admin-api/games/"))
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

                using (var response = await httpClient.GetAsync($"{ApiUrl}/admin-api/game/{id}"))
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



        private async Task<ResponseResult> getUsersList()
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                using (var response = await httpClient.GetAsync($"{ApiUrl}/setup/users/"))
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

        private async Task<ResponseResult> getUserById(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                var response = await httpClient.GetAsync($"{ApiUrl}/setup/user/{id}");
                var rolesResponse = await httpClient.GetAsync($"{ApiUrl}/setup/get-users-roles/{id}");

                string responseContent = await response.Content.ReadAsStringAsync();
                string rolesResponseContent = await rolesResponse.Content.ReadAsStringAsync();

                return new ResponseResult()
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    Response = responseContent
                };
                
            }
        }

        private async Task<ResponseResult> getUsersRoles(string id)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                var response = await httpClient.GetAsync($"{ApiUrl}/setup/get-users-roles/{id}");

                string responseContent = await response.Content.ReadAsStringAsync();

                return new ResponseResult()
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    Response = responseContent
                };

            }
        }

        private List<User> UserListConverter(string response)
        {
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        [VerifyRequest]
        public async Task<IActionResult> RevokeRole(string userId, string role)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(new RoleDTO() { Id = userId, Role = role}), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"{ApiUrl}/setup/revoke-role/", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine();
                }

                return Redirect($"~/user/{userId}");
            }
        }

        [VerifyRequest]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(new RoleDTO() { Id = userId, Role = role }), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"{ApiUrl}/setup/assign-role/", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine();
                }

                return Redirect($"~/user/{userId}");
            }
        }


        [VerifyRequest]
        public async Task<IActionResult> EditGame(Game game, IFormFile? gameFile)
        {
            if (!ModelState.IsValid)
            {
                return Redirect($"~/edit/{game.Id}");
            }

            game.LastUpdated = DateTime.Now;

            if (gameFile != null)
            {
                Tuple<bool, string> result = await _uploadService.UploadFile(gameFile, game);
                if (result.Item1)
                {
                    game.FilePath = result.Item2;
                }
            }

            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PatchAsync($"{ApiUrl}/admin-api/update/{game.Id}", content))
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

        [VerifyRequest]
        public async Task<IActionResult> AddGame(Game game, IFormFile? gameFile)
        {
            if (!ModelState.IsValid)
            {
                return Redirect($"~/add");
            }

            game.Featured = DateTime.Now;
            game.LastUpdated = DateTime.Now;
            game.FilePath = "none";
            game.Id = null;

            if (gameFile != null)
            {
                Tuple<bool, string> result = await _uploadService.UploadFile(gameFile, game);
                if (result.Item1)
                {
                    game.FilePath = result.Item2;
                }
            }

            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync($"{ApiUrl}/admin-api/add-game/", content))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return Redirect("~/games");
                    }

                    return Redirect($"~/add");
                }
            }
        }

        [VerifyRequest]
        public async Task<IActionResult> DeleteGame(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var token = JsonConvert.DeserializeObject<TokenDTO>(HttpContext.Session.GetString("JWT"));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.Token);

                using (var response = await httpClient.DeleteAsync($"{ApiUrl}/admin-api/delete/{id}"))
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    return Redirect("~/games");
                }
            }
        }
        #endregion
    }
}
