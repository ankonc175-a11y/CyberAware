using CyberAware.DataAccess.Entities;

namespace CyberAware.DataAccess.Repositories;

public interface IModuleRepository
{
    Task<List<Module>> GetAllPublishedAsync();
    Task<Module?> GetByIdAsync(int moduleId);
    Task<List<Module>> GetByCreatorAsync(int userId);
    Task<Module> AddAsync(Module module);
    Task UpdateAsync(Module module);
    Task DeleteAsync(int moduleId);
}