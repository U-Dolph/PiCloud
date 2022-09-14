using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PiCloudDashboard.Models;
using System.Diagnostics;
using System.Text;

namespace PiCloudDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
                            HttpContext.Session.SetString("token", jwt.Token);
                            HttpContext.Session.SetString("refreshToken", jwt.RefreshToken);
                            return Redirect("~/Dashboard/Index");
                        }
                    }

                    //return Redirect("~/Home/Index");
                    return null;
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}