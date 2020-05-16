using System;
using System.Linq;

namespace Pagination
{
    public class PageFactory : IPageFactory
    {
        public PagedViewModel<T> Paginate<T>(IQueryable<T> allItems, PageRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            var counter = new PageCounter((uint)request.PageNumber
                               , (byte)request.PageSize
                               , (uint)(allItems?.Count() ?? 0));

            var vm = new PagedViewModel<T>(counter, allItems);

            return vm;
        }

    }
}