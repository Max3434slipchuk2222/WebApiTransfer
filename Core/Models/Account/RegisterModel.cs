using Microsoft.AspNetCore.Http;

namespace Core.Models.Account;

public class RegisterModel
{
	public string Email { get; set; } = null!;
	public string Password { get; set; } = null!;
	public string FirstName { get; set; } = null!;
	public string LastName { get; set; } = null!;
	public string Phone { get; set; } = null!;
	public IFormFile? Image { get; set; }
}
