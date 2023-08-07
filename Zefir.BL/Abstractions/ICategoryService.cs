using Zefir.BL.Contracts.CategoryDto;
using Zefir.Core.Entity;

namespace Zefir.BL.Abstractions;

public interface ICategoryService
{
    public Task<List<Category>> GetAllCategories();
    public Task<Category> GetCategoryByName(string name);
    public Task<Category> CreateNewCategory(CreateCategoryServiceDto dto);
    public Task<Category> UpdateCategory(string name, UpdateCategoryServiceDto dto);
    public Task<bool> DeleteCategory(string name);
}