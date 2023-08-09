using Microsoft.EntityFrameworkCore;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.CategoryDto;
using Zefir.Common.Errors;
using Zefir.Core.Entity;
using Zefir.DAL;

namespace Zefir.BL.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _appDbContext;

    public CategoryService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Category>> GetAllCategories()
    {
        var categories = await _appDbContext.Categories.ToListAsync();
        return categories;
    }

    public async Task<Category> GetCategoryByName(string name)
    {
        var category = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (category is null) throw new ServiceNotFoundError("Can't find category with such name");
        return category;
    }

    public async Task<Category> CreateNewCategory(CreateCategoryServiceDto dto)
    {
        var candidate = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Name.Equals(dto.Name));
        if (candidate is not null) throw new ServiceBadRequestError(("Name", "Error with such name already exists"));

        candidate = new Category(dto.Name, dto.Description);

        await _appDbContext.Categories.AddAsync(candidate);
        await _appDbContext.SaveChangesAsync();

        return candidate;
    }

    public async Task<Category> UpdateCategory(string name, UpdateCategoryServiceDto dto)
    {
        var errors = new List<(string, string)>();
        var candidate = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Name.Equals(name));
        if (candidate is null) throw new ServiceNotFoundError("Can't find category with such name");
        if (string.IsNullOrWhiteSpace(dto.Name)) errors.Add(("Name", "Name can't be null or empty"));
        if (string.IsNullOrWhiteSpace(dto.Description))
            errors.Add(("Description", "Description can't be null or empty"));

        if (errors.Count > 0) throw new ServiceBadRequestError(errors.ToArray());

        candidate.Name = dto.Name;
        candidate.Description = dto.Description;
        await _appDbContext.SaveChangesAsync();
        return candidate;
    }

    public async Task<bool> DeleteCategory(string name)
    {
        var candidate = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.Name.Equals(name));
        if (candidate is null) return false;
        _appDbContext.Categories.Remove(candidate);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}
