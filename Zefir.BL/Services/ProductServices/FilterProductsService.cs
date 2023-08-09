using Microsoft.EntityFrameworkCore;
using Zefir.Common.Errors;
using Zefir.Core.Entity;
using Zefir.DAL;

namespace Zefir.BL.Services.ProductServices;

public class FilterProductsService
{
    private readonly AppDbContext _appDbContext;

    public FilterProductsService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IQueryable<Product>> FilterByCategory(IQueryable<Product> productsQuery, string categoryName)
    {
        categoryName = categoryName.Replace(" ", "");
        if (string.IsNullOrWhiteSpace(categoryName)) return productsQuery;

        return productsQuery.Where(p => p.Category.Name.Contains(categoryName));
    }

    public async Task<IQueryable<Product>> FilterByThematic(IQueryable<Product> productsQuery, string thematicName)
    {
        thematicName = thematicName.Replace(" ", "");
        if (string.IsNullOrWhiteSpace(thematicName)) return productsQuery;

        var thematic = await _appDbContext.Thematics.Include(thematic => thematic.Products)
            .FirstOrDefaultAsync(t => t.Name.Contains(thematicName));
        if (thematic is null) throw new ServiceBadRequestError((nameof(thematicName), "no such thematic"));

        return productsQuery.Where(p => thematic.Products.Any(product => product.Id == p.Id));
    }
}
