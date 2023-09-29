using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace BookShop.Areas.Admin.Services;

public class EmailSenderService : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress("twitterassistantbot@gmail.com");
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = htmlMessage;

        SmtpClient client = new SmtpClient();
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential("twitterassistantbot", "ekjktlqjlhvhkchl");
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        await client.SendMailAsync(mailMessage);
    }
}
