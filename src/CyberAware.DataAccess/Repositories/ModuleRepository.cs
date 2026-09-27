using CyberAware.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CyberAware.DataAccess.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly AppDbContext _context;

    public ModuleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Module>> GetAllPublishedAsync()
    {
        return await _context.Modules
            .Where(m => m.IsPublished)
            .ToListAsync();
    }

    public async Task<Module?> GetByIdAsync(int moduleId)
    {
        return await _context.Modules
            .Include(m => m.ContentSections.OrderBy(c => c.OrderIndex))
            .Include(m => m.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(m => m.ModuleID == moduleId);
    }

    public async Task<List<Module>> GetByCreatorAsync(int userId)
    {
        return await _context.Modules
            .Where(m => m.CreatedBy == userId)
            .ToListAsync();
    }

    public async Task<Module> AddAsync(Module module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
        return module;
    }

    public async Task UpdateAsync(Module module)
    {
        module.UpdatedAt = DateTime.UtcNow;
        _context.Modules.Update(module);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int moduleId)
    {
        var module = await _context.Modules.FindAsync(moduleId);
        if (module != null)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }
    }
}