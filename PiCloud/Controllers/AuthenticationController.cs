using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PiCloud.Data;
using PiCloud.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Identity.UI;

namespace PiCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationController(UserManager<IdentityUser> userManager, /*JwtConfig jwtConfig*/ IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            //_jwtConfig = jwtConfig;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDto)
        {
            Log.Information("Auth ");
            //Validate request
            if (ModelState.IsValid)
            {
                //Check user already exists
                var user_exists = await _userManager.FindByEmailAsync(userDto.Email);

                if (user_exists != null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exists!"
                        }
                    });
                }

                // Else create a new user
                var new_user = new IdentityUser()
                {
                    Email = userDto.Email,
                    UserName = userDto.Username
                };

                var is_created = await _userManager.CreateAsync(new_user, userDto.Password);

                if (is_created.Succeeded)
                {
                    await _userManager.AddToRoleAsync(new_user, "user");
                    return Ok(new AuthResult()
                    {
                        Result = true,
                    });
                }

                //Else throw error
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Unknown error!"
                    }
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDto)
        {
            if (ModelState.IsValid)
            {
                var existing_user = await _userManager.FindByNameAsync(userDto.Username);

                if (existing_user == null)
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid payload"
                        }
                    });

                var is_correct = await _userManager.CheckPasswordAsync(existing_user, userDto.Password);

                if (!is_correct)
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid payload"
                        }
                    });

                var token = await generateJwt(existing_user);

                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Invalid payload"
                }
            });
        }

        private async Task<string> generateJwt(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Key").Value);

            var claims = await getValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.Now.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }

        private async Task<List<Claim>> getValidClaims(IdentityUser user)
        {
            var _options = new IdentityOptions();
            var _claims = new List<Claim> 
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            _claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    _claims.Add(new Claim(ClaimTypes.Role, userRole));
                    var roleClaims = await _roleManager.GetClaimsAsync(role);

                    foreach (var roleClaim in roleClaims)
                        _claims.Add(roleClaim);
                    
                }
            }

            return _claims;
        }
    }
}
