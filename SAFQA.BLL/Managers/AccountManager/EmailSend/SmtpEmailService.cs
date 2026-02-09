using Microsoft.Extensions.Configuration;
using SAFQA.BLL.Managers.AccountManager.SendEmail;
using System.Net;
using System.Net.Mail;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var smtp = _config.GetSection("EmailSettings");

        string host = smtp["Host"];
        int port = int.Parse(smtp["Port"]);
        string fromEmail = smtp["From"];
        string userName = smtp["UserName"];
        string password = smtp["Password"];

        if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            throw new Exception("SMTP configuration is missing in appsettings.json");

        if (string.IsNullOrWhiteSpace(to))
            throw new Exception("Recipient email is null!");

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(to);

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        await client.SendMailAsync(message);
    }

    public async Task SendOtpEmailAsync(string to, string otp)
    {
        string subject = "Your OTP Code";
        string body = $"Your OTP is: {otp}";
        await SendEmailAsync(to, subject, body);
    }
}
