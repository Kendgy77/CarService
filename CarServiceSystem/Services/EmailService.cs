using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System;

namespace CarServiceSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                Console.WriteLine($"Próba wysłania emaila na {email}");
                Console.WriteLine($"Używam SMTP: {_config["Email:Host"]}:{_config["Email:Port"]}");

                var mailMessage = new MimeMessage();
                mailMessage.From.Add(new MailboxAddress(
                    _config["Email:FromName"],
                    _config["Email:FromEmail"]));
                mailMessage.To.Add(new MailboxAddress("", email));
                mailMessage.Subject = subject;
                mailMessage.Body = new TextPart("plain") { Text = message };

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _config["Email:Host"],
                    int.Parse(_config["Email:Port"]),
                    SecureSocketOptions.StartTlsWhenAvailable);

                Console.WriteLine("Połączenie z SMTP udane");

                await client.AuthenticateAsync(
                    _config["Email:Username"],
                    _config["Email:Password"]);

                Console.WriteLine("Autoryzacja udana");

                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);

                Console.WriteLine($"Email wysłany na {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wysyłki emaila: {ex.ToString()}");
                throw;
            }
        }
    }
}

