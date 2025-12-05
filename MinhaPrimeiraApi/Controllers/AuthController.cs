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

        return Unauthorized(new {Status = "Error", Message = "Invalid email or password"});
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
            return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "User already exists with this email" });
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

}