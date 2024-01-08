using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MBM.Common.Models.Api.Request;

namespace Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;

        public TokenController(IConfiguration configuration)
        {
            _issuer = configuration["Jwt:Issuer"]!;
            _audience = configuration["Jwt:Audience"]!;
            _key = configuration["Jwt:Key"]!;
        }

        [HttpPost("generate")]
        public IActionResult GenerateToken([FromBody] TokenRequest model)
        {
            if (model.TokenSecretKey != _key)
            {
                return BadRequest("Invalid key provided!");
            }

            var token = BuildToken(model.UserId, model.Username);
            return Ok(new { token });
        }

        private string BuildToken(string userId, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                // Add more claims as needed
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}