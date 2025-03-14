using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockApi.Interfaces;
using StockApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController (IUserRepository userRepository,IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }



        [Authorize(Roles = "Admin")]  
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }




        [Authorize(Roles = "Admin")] 
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            return Ok(user);
        }



        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<User>> SignUp([FromBody] RegisterRequest register)
        { 
            var existingUser = await _userRepository.GetUserByEmail(register.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists with this email");
            }

            var newUser = new User
            {
                Email = register.Email,
                Password = register.Password,
                Roles = new List<string> { "User" }
            };
            await _userRepository.Register(newUser);
            return Ok(new { message = "User registered succesfully", user = newUser });
        }



        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult<string>> SignIn([FromBody] LoginRequest request)
        {
            var user = await _userRepository.Login(request.Email,request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");              
            }
            var token = GenerateJwtToken(user);
#pragma warning disable IDE0037 // Use inferred member name
            return Ok(new
            {
                message = "Login successful",
                token = token,
                roles = user.Roles
            });
#pragma warning restore IDE0037 // Use inferred member name
        }



        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                  _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<bool> UpdateUser(string userId, User updateUser)
        {
            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                return false;
            }

            return await _userRepository.UpdateUser(userId, updateUser);
        }



        [Authorize(Roles = "Admin")] 
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteUser(string userId)
        {
            return Ok( await _userRepository.DeleteUser(userId));
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("update-role/{userId}")]
        public async Task<ActionResult<User>> UpdateUserRole(string userId, [FromBody]string newRole)
        {
            try
            {
                var updateUser = await _userRepository.UpdateUserRole(userId, newRole);
                var newToken = GenerateJwtToken(updateUser);
                return Ok(new { message = "User role updated successfully",newtoken = newToken });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
