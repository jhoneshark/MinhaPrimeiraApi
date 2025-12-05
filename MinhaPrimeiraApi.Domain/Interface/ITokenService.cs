using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface ITokenService
{
    string GenerateAccessToken(Users user);
    JwtSecurityToken CreateJwtSecurityTokenObject(IEnumerable<Claim> claims, IConfiguration config);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration config);
}