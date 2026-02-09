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
        var smtp = _config.GetSection("SmtpSettings");

        var message = new MailMessage
        {
            From = new MailAddress(smtp["UserName"], "SAFQA App"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(to);

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]))
        {
            Credentials = new NetworkCredential(
                smtp["UserName"],
                smtp["Password"]
            ),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        await client.SendMailAsync(message);
    }
    public async Task SendOtpEmailAsync(string to, string otp)
    {
        var subject = "Your OTP Code";
        var body = $"Your OTP is: {otp}";
        await SendEmailAsync(to, subject, body);
    }

}