namespace Zefir.Common.Helpers;

public class PaginationHelper
{
    private readonly int _countByPage;

    public PaginationHelper(int countByPage)
    {
        _countByPage = countByPage;
    }

    public IQueryable<T> GetPagedItems<T>(IQueryable<T> items, int currentPage = 0)
    {
        return items.Skip(currentPage).Take(_countByPage);
    }

    public int ComputeCountOfPages(int totalObjects)
    {
        return totalObjects / _countByPage;
    }
}
