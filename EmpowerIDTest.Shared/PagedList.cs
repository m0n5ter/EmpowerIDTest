namespace EmpowerIDTest.Shared;

public class PagedList<T> : List<T>
{
    public PagedList(IList<T> list, int fullCount, int filteredCount, int pageSize, int pageNumber) : base(list)
    {
        FullCount = fullCount;
        FilteredCount = filteredCount;
        PageSize = pageSize;
        PageNumber = pageNumber;
        PageCount = fullCount / pageSize + (fullCount % pageSize > 0 ? 1 : 0);
    }

    public int PageSize { get; }

    public int PageNumber { get; }

    public int PageCount { get; }

    public int FullCount { get; }

    public int FilteredCount { get; }
}