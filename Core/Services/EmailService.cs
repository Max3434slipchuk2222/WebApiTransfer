using System.Net;
using System.Net.Mail;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Core.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
	public async Task SendEmailAsync(string admin, string subject, string body)
	{
		var server = configuration["EmailSettings:SmtpServer"];
		var port = int.Parse(configuration["EmailSettings:Port"]!);
		var fromEmail = configuration["EmailSettings:SenderEmail"];
		var password = configuration["EmailSettings:SenderPassword"];

		using var client = new SmtpClient(server, port)
		{
			Credentials = new NetworkCredential(fromEmail, password),
			EnableSsl = true
		};

		var mailMessage = new MailMessage
		{
			From = new MailAddress(fromEmail!, "TravelApp Notification"),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		mailMessage.To.Add(admin);

		try
		{
			await client.SendMailAsync(mailMessage);
			Console.WriteLine($"Email sent to {admin} successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error sending email: {ex.Message}");
		}
	}
}