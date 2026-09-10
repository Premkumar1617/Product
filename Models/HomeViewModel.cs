namespace SJBRAssociatesWeb.Models;

public class HomeViewModel
{
    public List<TeamMember> Team { get; set; } = new();
    public List<ServiceItem> Services { get; set; } = new();
    public FirmContact Contact { get; set; } = new();
}
