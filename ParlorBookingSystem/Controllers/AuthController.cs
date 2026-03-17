using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ParlorBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // --- 1. REGISTER: Create Auntie's Admin Account ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) return BadRequest("User already exists!");

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest("User creation failed!");

            // Create 'Admin' role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            // Assign Auntie to Admin role
            await _userManager.AddToRoleAsync(user, "Admin");

            return Ok(new { Message = "Admin Registered Successfully!" });
        }

        // --- 2. LOGIN: Auntie gets her JWT Token ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // 1. Prepare the key
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForAuntiesParlor123!"));

                // 2. Define the algorithm (This is what the error was asking for)
                var creds = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

                // 3. Create the token
                var token = new JwtSecurityToken(
                    issuer: "AuntiesParlor",
                    audience: "AuntiesParlorUsers",
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: creds // Use 'signingCredentials' instead of 'signingKey'
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }

    // Small classes to hold the data coming from the user
    public class RegisterModel { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
    public class LoginModel { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
}