using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PiCloud.Data;
using PiCloud.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace PiCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly TokenValidationParameters _validationParameters;

        public AuthenticationController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            TokenValidationParameters validationParameters)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _validationParameters = validationParameters;
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

                return Ok(token);
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

        private async Task<AuthResult> generateJwt(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Key").Value);

            var claims = await getValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomStringGenerator(32),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(12),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResult()
            {
                Result = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(tokenRequest);

                if (result == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid tokens"
                        }
                    });
                }

                return Ok(result);
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>
                {
                    "Invalid parameters"
                }
            });
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequestDTO tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _validationParameters.ValidateLifetime = false; // for testing

                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (!result)
                        return null;
                }

                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.Now)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Expired token!"
                        }
                    };
                }

                var storedToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid token!"
                        }
                    };
                }

                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid token!"
                        }
                    };
                }

                if (storedToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid token!"
                        }
                    };
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid token!"
                        }
                    };
                }

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Expired token!"
                        }
                    };
                }

                storedToken.IsUsed = true;
                _dbContext.RefreshTokens.Update(storedToken);
                await _dbContext.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await generateJwt(dbUser);
            }
            catch (Exception)
            {
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>
                    {
                        "Unknown error!"
                    }
                };
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        private string RandomStringGenerator(int n)
        {
            var rnd = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new string(Enumerable.Repeat(chars, n).Select(s => s[rnd.Next(s.Length)]).ToArray());
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
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
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
