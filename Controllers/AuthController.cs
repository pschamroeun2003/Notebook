using Microsoft.AspNetCore.Mvc;
using NoteTakingApp.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data.SqlClient;
using NoteTakingApp.DTOs;
using BCrypt.Net;
using System.Threading.Tasks;
using System;

namespace NoteTakingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthController(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userDto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var existingUser  = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE Username = @Username", new { Username = userDto.Username });

                if (existingUser  != null)
                    return BadRequest(new { status = false, message = "Username already exists" });
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
                };
                var insertQuery = "INSERT INTO Users (Username, Email, PasswordHash) OUTPUT INSERTED.Id VALUES (@Username, @Email, @PasswordHash)";
                user.Id = await connection.ExecuteScalarAsync<int>(insertQuery, user); 
                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                var userResponse = new UserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token,
                    RefreshToken = refreshToken
                };

                return Ok(new { status = true, message = "User  registered successfully", user = userResponse });
            }
        }

       [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
              
                var dbUser  = await connection.QueryFirstOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE Email = @Email", new { Email = userDto.Email });
                if (dbUser  == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, dbUser .PasswordHash))
                    return Unauthorized(new { status = false, message = "Invalid credentials" });
                var token = GenerateJwtToken(dbUser );
                var refreshToken = GenerateRefreshToken();
                var userResponse = new UserResponseDTO
                {
                    Id = dbUser .Id,
                    Username = dbUser .Username,
                    Email = dbUser .Email,
                    Token = token,
                    RefreshToken = refreshToken 
                };
                return Ok(new { status = true, user = userResponse });
            }
        }
        private string GenerateJwtToken(User user)
        {
            var key = _config["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("JWT Key is missing in configuration.");
            }
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            var refreshToken = Convert.ToBase64String(randomNumber);
            Console.WriteLine("Generated Refresh Token: " + refreshToken);
            return refreshToken;
        }
    }
}
