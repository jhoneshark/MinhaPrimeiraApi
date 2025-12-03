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
    private readonly IUserRepository _userRepository;

    public AuthController(ITokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO loginModel)
    {
        throw  new NotImplementedException();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO model)
    {
        var userExists = await _userRepository.GetUserByEmailAsync(model.Email);

        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "User already exists with this email" });
        }
        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var user = new Users()
        {
            CprOrCnpj = model.CprOrCnpj,
            Email = model.Email,
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