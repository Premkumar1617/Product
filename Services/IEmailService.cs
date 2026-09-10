namespace SJBRAssociatesWeb.Services;

public interface IEmailService
{
    Task SendEnquiryNotificationAsync(string name, string email, string phone, string message);
}
