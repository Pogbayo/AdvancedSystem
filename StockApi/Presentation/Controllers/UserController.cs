using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StockApi.Domain.Interfaces;
using StockApi.Domain.Entities;
using StockApi.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StockApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController (IUserRepository userRepository,IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            if (users == null || !users.Any())
            {
                return NotFoundResponse<List<User>>(new List<string> { "No users found" },"users not fetched successfully");
            }
            return Success<List<User>>(users, "Users fetched successfully");
        }


        [Authorize(Roles = "Admin")] 
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUserById(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user is null)
            {
                return NotFoundResponse<User>(
                     new List<string> { "User not found" },
                     "Failed to fetch user"
                      );
            }
            return Success<User>(user,"User fetched successfully");
        }


        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<User>>> SignUp([FromBody] RegisterRequest register)
        { 
            var existingUser = await _userRepository.GetUserByEmail(register.Email);
            if (existingUser != null)
            {
                return NotFoundResponse<User>(
                    new List<string> { "User already exists" },
                    "Failed to register user"
                    );
            }

            var newUser = new User
            {
                Email = register.Email,
                Password = register.Password,
                Roles = new List<string> { "User" }
            };
            await _userRepository.Register(newUser);
            return Success<User>(newUser, "User created successfully");
        }


        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<ActionResult<ApiResponse<string>>> SignIn([FromBody] LoginRequest request)
        {
            var user = await _userRepository.Login(request.Email,request.Password);
            if (user == null)
            {
                return UnAuthorizedResponse<string>(
                    new List<string> { "Invalid email or password" },
                    "Failed to signin user"
                    );              
            }
            var token = GenerateJwtToken(user);
#pragma warning disable IDE0037 // Use inferred member name
            return Success<string>(token,"User logged in successfully");
#pragma warning restore IDE0037 // Use inferred member name
        }


        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

#pragma warning disable CS8604 // Possible null reference argument.
            var claims = new List<Claim>
            {
              new Claim(ClaimTypes.NameIdentifier, user.Id), 
              new Claim(ClaimTypes.Email, user.Email),
            };
#pragma warning restore CS8604 // Possible null reference argument.

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


        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateUser(string userId, User updateUser)
        {
            var existingUser = await _userRepository.GetUserById(userId);
            if (existingUser == null)
            {
                return NotFoundResponse<bool>(
                           new List<string> { "User not found" },
                           "Update failed"
                       );
            }
            var isUpdated = await _userRepository.UpdateUser(userId, updateUser);
            if (!isUpdated)
            {
                return Failure<bool>(
                    new List<string> { "Failed to update user" },
                    "Update operation failed"
                );
            }
            return Success<bool>(true, "User updated successfully");
        }




        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string userId)
        {
            var existingUser = await _userRepository.GetUserById(userId);

            if (existingUser == null)
            {
                return NotFoundResponse<bool>(
                           new List<string> { "User not found" },
                           "Deletion failed"
                       );
            }

            var isUpdated = await _userRepository.DeleteUser(userId);

            if (!isUpdated)
            {
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    new List<string> { "Failed to delete user" },
                    "UpDeletedate operation failed"
                ));
            }

            return Success<bool>(true, "User deleted successfully");
        }




        [HttpPut("update-role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUserRole(string userId, [FromBody] string newRole)
        {
            var updatedUser = await _userRepository.UpdateUserRole(userId, newRole);
            if (updatedUser == null)
            {
                return NotFoundResponse<string>(new List<string> { "User not found or role update failed" }, "Role update failed");
            }

            var newToken = GenerateJwtToken(updatedUser);
            return Success(newToken, "User role updated successfully");
        }

    }
}
