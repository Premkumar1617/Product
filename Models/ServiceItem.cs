namespace SJBRAssociatesWeb.Models;

public class ServiceItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Bootstrap Icons class name, e.g. "bi-clipboard-check".</summary>
    public string Icon { get; set; } = "bi-briefcase";
}
