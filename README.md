This library calculates the page number, selects the corresponding page, and finally returns the page and the related pagination metadata for futher useage as HTTP headrs or for HATEOAS link generation by providing a simple API. A `PageFactory` which accepts an `IQueryable<YourViewModel>` and a `PageRequest`, and returns a `PagedViewModel<YourViewModel>`

## How to use it
Install [this](https://www.nuget.org/packages/Vlerx.Pagination/) nuget package.

Let's see the following query:
```cs
public IQueryable<User> ListUsers()
{
  return DbContext.Users;
}
```
We want to paginate it as well as returning all the related pagination metadata.
```cs
public PagedViewModel<Users> ListUsers(PageRequest request)
{
   pageFactory.Paginate(dbContext.Users, request);
}
readonly IPageFactory pageFactory;
```
Let's call it.
```cs
var page = ListUsers(new PageRequest{PageNumber = 1, PageSize = 10});
```
this is what it returns:
```cs
public class PagedViewModel<T>
{
    public IQueryable<T> ItemsInThePage { get; }
    public PageCounter Counter { get; }
}
```
The `ItemsInThePage` is the result of slicing the requested page from the query parameter.
And the page counter is the metadata of the page:
```cs
public class PageCounter
{
    public uint Number { get; }
    public byte Size { get; }
    public uint TotalItemCount { get; }

    public uint TotalPageCount { get; }
    public bool HasNext { get; }
    public bool HasPrevious { get; }
}
```
The `IPageFactory` can be easily injected by any IOC:
```cs
services.AddSingleton<IPageFactory, PageFactory>();
```
