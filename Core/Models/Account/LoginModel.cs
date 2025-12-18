namespace Core.Models.Account;

public class LoginModel
{
	/// <summary>
	/// Електронна пошта користувача
	/// </summary>
	/// <example>maxslipchuk11@gmail.com</example>
	public string Email { get; set; } = null!;
	/// <summary>
	/// Пароль користувача
	/// </summary>
	/// <example>Admin123</example>
	public string Password { get; set; } = null!;
}
