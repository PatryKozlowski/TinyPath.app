namespace TinyPath.Domain.Entities.TinyPath;

public class GetLinks
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool IsCustom { get; set; }
}