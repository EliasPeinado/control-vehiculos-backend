namespace ControlVehiculos.Models.DTOs;

public class PageMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public bool HasNext { get; set; }
}

public class PagedResponse<T>
{
    public PageMeta Meta { get; set; } = null!;
    public List<T> Items { get; set; } = new();
}
