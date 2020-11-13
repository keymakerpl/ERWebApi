using ERService.Infrastructure.Helpers;
using ERWebApi.SQLDataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ERWebApi.Models;

namespace ERWebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authorization")]
    [Produces("application/json", "application/xml")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthorizationController(IConfiguration configuration, IUsersRepository usersRepository, IPasswordHasher passwordHasher)
        {
            _configuration = configuration;
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Create token for web api authorization purpose.
        /// </summary>
        /// <param name="loginInfo">Login and password information.</param>
        /// <returns>Ok token string.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateToken([FromBody] LoginInfoDto loginInfo)
        {
            if (String.IsNullOrWhiteSpace(loginInfo.Login) || String.IsNullOrWhiteSpace(loginInfo.Password))
                return BadRequest();

            var user = await _usersRepository.GetByLoginAsync(loginInfo.Login);
            if (user == null)
                return NotFound();

            if (!_passwordHasher.VerifyPassword(loginInfo.Password, user.PasswordHash, user.Salt))
                return BadRequest("Invalid credidentials");
            
            Claim[] claims = GetClaims(user);
            string token = GetToken(claims);

            return Ok(token);
        }

        private string GetToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn);

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        private Claim[] GetClaims(ERService.Business.User user)
        {
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Login", user.Login),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? ""),
                new Claim("PhoneNumber", user.PhoneNumber ?? "")
            };
        }
    }
}
