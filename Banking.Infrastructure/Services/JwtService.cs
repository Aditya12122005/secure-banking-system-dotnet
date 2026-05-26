using Banking.Application.Interfaces;
using Banking.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Banking.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var secret = _configuration["JwtSettings:Secret"];

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

    new Claim(JwtRegisteredClaimNames.Email, user.Email),

    new Claim(ClaimTypes.Role, user.Role),

    new Claim("FullName", user.FullName)
};

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],

            audience: _configuration["JwtSettings:Audience"],

            claims: claims,

            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(
                    _configuration["JwtSettings:ExpiryMinutes"])),

            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}