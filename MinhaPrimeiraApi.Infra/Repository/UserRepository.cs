using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Infra.Context;

namespace MinhaPrimeiraApi.Domain.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Users> CreateUserAsync(Users user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserRefreshToken(int userId, string refreshToken, DateTime expiryTime)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiryTime;
            user.UpdatedAt = DateTime.UtcNow;
            
            _context.Users.Update(user); 
            await _context.SaveChangesAsync();
        }
    }
}