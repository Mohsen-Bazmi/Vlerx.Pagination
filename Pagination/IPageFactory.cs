using System.Collections.Generic;
using System.Linq;

namespace Pagination
{
    public interface IPageFactory
    {
        PagedViewModel<T> Paginate<T>(IQueryable<T> allItems, PageRequest request);
    }
}