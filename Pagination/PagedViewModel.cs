using System;
using System.Linq;

namespace Pagination
{
    public class PagedViewModel<T>
    {
        public PageCounter Counter { get; }
        public IQueryable<T> ItemsInThePage { get; }
        public PagedViewModel(PageCounter counter, IQueryable<T> allItems)
        {
            Counter = counter ?? throw new ArgumentNullException(nameof(counter));

            ItemsInThePage = null == allItems
                           ? Array.Empty<T>().AsQueryable()
                           : SlicePage(allItems, Counter);
        }

        IQueryable<T> SlicePage(IQueryable<T> allItems, PageCounter page)
        => allItems.Skip((int)(page.Size * (page.Number - 1))).Take(page.Size);


    }
}