namespace SJBRAssociatesWeb.Models;

public class FirmContact
{
    public string FirmName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
    public string WhatsApp { get; set; } = string.Empty;
    public string IncorporatedOn { get; set; } = string.Empty;
}
