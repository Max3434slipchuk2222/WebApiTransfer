

namespace Core.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(string admin, string subject, string body);
	}
}
