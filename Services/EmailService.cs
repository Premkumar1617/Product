using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SJBRAssociatesWeb.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEnquiryNotificationAsync(string name, string email, string phone, string message)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = $"New website enquiry from {name}",
                Body = $"Name: {name}\nEmail: {email}\nPhone: {phone}\n\nMessage:\n{message}",
                IsBodyHtml = false
            };
            mail.To.Add(_settings.ToAddress);
            mail.ReplyToList.Add(new MailAddress(email));

            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            // Deliberately swallow: the enquiry is already safely saved to
            // enquiries.json by the time this runs. Don't let a flaky SMTP
            // server turn into a failed form submission for the visitor.
            _logger.LogError(ex, "Failed to send enquiry notification email for {Email}", email);
        }
    }
}
