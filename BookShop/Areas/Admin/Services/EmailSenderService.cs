using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookShop.Areas.Admin.Services;

public class EmailSenderService : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using SmtpClient client = new SmtpClient();

        client.Credentials = new NetworkCredential("twitterassistantbot", "AMIRREZA3439a");
        client.Host = "smtp.gmail.com";
        client.Port = 578;
        client.EnableSsl = true;

        using MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("twitterassistantbot@gmail.com");
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = htmlMessage;
        await client.SendMailAsync(mailMessage);
    }
}
