using MinhaPrimeiraApi.Domain.Models;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface IUserRepository
{
    Task<Users?> GetUserByEmailAsync(string email);
    Task<Users> CreateUserAsync(Users user);
}