using CyberAware.DataAccess.Entities;

namespace CyberAware.DataAccess.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<List<User>> GetAllAsync();
}