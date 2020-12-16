using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;


namespace NewApp.Models.Data
{
    public class EmailService
    {
       public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Myplan повідомлення", "marishka3006@ukr.net"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.ukr.net",465, true);
                await client.AuthenticateAsync("marishka3006@ukr.net", "sBITlgULXrU7YYxq");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}