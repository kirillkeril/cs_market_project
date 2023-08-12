using Microsoft.EntityFrameworkCore;
using Zefir.Common.Errors;
using Zefir.Core.Entity;
using Zefir.DAL;

namespace Zefir.Common.Helpers;

public class FilterProductsBuilder
{
    private IQueryable<Product> _productQuery;
    private readonly AppDbContext _appDbContext;

    public FilterProductsBuilder(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _productQuery = _appDbContext.Products;
    }

    public FilterProductsBuilder FilterBySearch(string search)
    {
        search = search.ToLower();
        if (string.IsNullOrWhiteSpace(search)) return this;
        _productQuery = _appDbContext.Products.Where(
            p =>
                p.Characteristics != null && (p.Name.ToLower().Contains(search) ||
                                              p.Description.ToLower().Contains(search) ||
                                              p.Characteristics.Any(c => c.Value.ToLower().Contains(search)))
        );
        return this;
    }

    public FilterProductsBuilder FilterByCategory(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return this;

        categoryName = categoryName.Replace(" ", "");
        _productQuery = _appDbContext.Products.Where(p => p.Category.Name.Contains(categoryName));
        return this;
    }

    public FilterProductsBuilder FilterByThematic(string thematicName)
    {
        if (string.IsNullOrWhiteSpace(thematicName)) return this;

        thematicName = thematicName.Replace(" ", "");

        var thematic = _appDbContext.Thematics.Include(thematic => thematic.Products)
            .FirstOrDefault(t => t.Name.Contains(thematicName));
        if (thematic is null) throw new ServiceBadRequestError((nameof(thematicName), "no such thematic"));

        _productQuery = _productQuery.Where(p => thematic.Products.Any(product => product.Id == p.Id));
        return this;
    }

    public IQueryable<Product> BuildQuery()
    {
        return _productQuery;
    }
}
