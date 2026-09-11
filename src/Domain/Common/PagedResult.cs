namespace CleanArchitecture.Domain.Common;

public sealed class PagedResult<T>
{
    public PagedResult(IReadOnlyCollection<T> items, int totalItems, int page, int pageSize)
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
    }

    public IReadOnlyCollection<T> Items { get; }

    public int TotalItems { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;
}
