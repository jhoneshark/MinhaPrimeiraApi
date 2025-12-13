using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinhaPrimeiraApi.Domain.DTOs;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(ITokenService tokenService, IConfiguration configuration, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO loginModel)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginModel.Email);
        if (user is not null && BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenValidityInMinutes = _configuration.GetValue<int>("JWT:RefreshTokenValidityInMinutes");
            var refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            await _userRepository.UpdateUserRefreshToken(user.Id, refreshToken, refreshTokenExpiryTime);

            return Ok(new
            {
                Status = "Success",
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = refreshTokenExpiryTime
            });
        }

        return Unauthorized(new { Status = "Error", Message = "Invalid email or password" });
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
    {
        string emailLower = model.Email.ToLower();
        string cpcOrCnpj = new string(model.CprOrCnpj.Where(char.IsDigit).ToArray());

        var userExists = await _userRepository.GetUserByEmailAsync(emailLower);

        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new { Status = "Error", Message = "User already exists with this email" });
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var user = new Users()
        {
            Name = model.Name,
            CprOrCnpj = cpcOrCnpj,
            Email = emailLower,
            RoleId = 1,
            PasswordHash = passwordHash,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        var result = await _userRepository.CreateUserAsync(user);

        return StatusCode(StatusCodes.Status201Created, new
        {
            Status = "Success", Message = "User created successfully!", UserId = result.Id
        });
    }

    [HttpPost]
    [Route("refreshToken")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        string? accessToken = tokenModel.AcessToken ?? throw new ArgumentNullException(nameof(tokenModel));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Invalid access token/refresh token");
        }

        var email = principal.FindFirst(ClaimTypes.Email).Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Invalid token claims");
        }

        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        DateTime existingExpiryTime = user.RefreshTokenExpiryTime ?? DateTime.UtcNow;

        user.RefreshToken = newRefreshToken;

        await _userRepository.UpdateUserRefreshToken(user.Id, newRefreshToken, existingExpiryTime);

        return Ok(new
        {
            Status = "Success",
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = existingExpiryTime
        });
    }

    [HttpPost]
    [Route("revokeToken")]
    [Authorize(Policy = "ExclusivePolicyOnly")]
    public async Task<IActionResult> RevokeToken(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        
        if (user is null) return BadRequest("Invalid user email");

        user.RefreshToken = null;
        await _userRepository.UpdateUserRefreshToken(user.Id, null, DateTime.UtcNow);
       
        return NoContent();
    }

}