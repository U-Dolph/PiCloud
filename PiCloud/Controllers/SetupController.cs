using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiCloud.Data;

namespace PiCloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public SetupController(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult AllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            // Check if role exists
            var roleExist = await _roleManager.RoleExistsAsync(name);

            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = name });

                if (roleResult.Succeeded)
                {
                    return Ok(new
                    {
                        result = $"Role {name} created"
                    });
                }

                return BadRequest(new { error = "Role cannot be created" });
            }

            return BadRequest(new {error = "Role already exists"});
        }

        [HttpGet]
        [Route("users")]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        [Route("assignRole")]
        public async Task<IActionResult> AssignRole(string email, string roleName)
        {
            // Check if role exists
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) { return BadRequest(new { error = "User does not exist!"}); }
            if (!roleExist) { return BadRequest(new { error = "Role does not exist!" }); }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            
            if (result.Succeeded) return Ok(new { result = $"{user.UserName} added to {roleName}" });

            return BadRequest(new { error = $"{user.UserName} cannot be added to {roleName}" });
        }

        [HttpGet]
        [Route("getuserroles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) { return BadRequest(new { error = "User does not exist!" }); }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpPost]
        [Route("revokeRole")]
        public async Task<IActionResult> RevokeRole(string email, string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) { return BadRequest(new { error = "User does not exist!" }); }
            if (!roleExist) { return BadRequest(new { error = "Role does not exist!" }); }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded) return Ok(new { result = $"{user.UserName} removed from {roleName}" });

            return BadRequest(new { error = $"{user.UserName} cannot be removed from {roleName}" });
        }
    }
}
