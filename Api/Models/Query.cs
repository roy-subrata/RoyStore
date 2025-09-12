namespace Api.Models;

public interface IPaging<T>
{
    List<T> Data { get; set; }
    int Total { get; set; }
    int Page { get; set; }
    int PageSize { get; set; }
}

public class Paging<T> : IPaging<T> where T : class
{
    public List<T> Data { get; set; } = [];
    public int Total { get; set; } = 0;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public  class Query
{
    private int _page = 1;
    private int _pageSize = 50;

    public string? Search { get; set; } = string.Empty;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > 100 ? 50 : value;
    }
}


