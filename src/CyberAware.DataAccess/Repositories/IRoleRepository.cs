using CyberAware.DataAccess.Entities;

namespace CyberAware.DataAccess.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
    Task<Role?> GetByIdAsync(int roleId);
    Task<List<Role>> GetAllAsync();
}