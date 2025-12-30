using Core.Interfaces;
using Core.Models.Account;
using Core.Services;
using Domain.Entities.Idenity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(UserManager<UserEntity> userManager,IUserService userService, IJwtTokenService jwtTokenService, GoogleService googleService, SignInManager<UserEntity> signInManager, IImageService imageService) : ControllerBase
{
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromForm] RegisterModel model)
	{
		var repeatUser = await userManager.FindByEmailAsync(model.Email);
		if (repeatUser != null)
		{
			return BadRequest(new { Message = "Користувач з такою поштою вже існує" });
		}

		string imageName = string.Empty;
		if (model.Image != null)
		{
			imageName = await imageService.UploadImageAsync(model.Image);
		}
		var user = new UserEntity
		{
			FirstName = model.FirstName,
			LastName = model.LastName,
			Email = model.Email,
			UserName = model.Email,
			PhoneNumber = model.Phone,
			Image = imageName,
			EmailConfirmed = true 
		};

		var result = await userManager.CreateAsync(user, model.Password);

		if (!result.Succeeded)
		{
			if (!string.IsNullOrEmpty(imageName))
			{
				imageService.DeleteImage(imageName);
			}
			return BadRequest(result.Errors);
		}
		var token = await jwtTokenService.CreateAsync(user);
		return Ok(new { token });
	}
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginModel model)
	{
		var user = await userManager.FindByEmailAsync(model.Email);
		if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
		{
			return Unauthorized("Invalid email or password.");
		}
		var token = await jwtTokenService.CreateAsync(user);
		return Ok(new { token });
	}

	[HttpPost("google-login")]
	public async Task<IActionResult> GoogleLogin([FromBody] GoogleLogin model)
	{
		var payload = await googleService.VerifyGoogleToken(model.Token);
		if (payload == null) return BadRequest("Invalid Google Token");

		var info = new UserLoginInfo("Google", payload.Subject, "Google");

		var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

		if (user == null)
		{
			user = await userManager.FindByEmailAsync(payload.Email);
			if (user == null)
			{
				user = new UserEntity
				{
					UserName = payload.Email,
					Email = payload.Email,
					FirstName = payload.GivenName,
					LastName = payload.FamilyName,
					Image = payload.Picture,
					EmailConfirmed = true
				};
				var createResult = await userManager.CreateAsync(user);
				if (!createResult.Succeeded) return BadRequest(createResult.Errors);
			}
			await userManager.AddLoginAsync(user, info);
				
		}
		var jwtToken =  await jwtTokenService.CreateAsync(user);
		return Ok(new { token = jwtToken });
	}

	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();
		return Ok(new { Message = "Logged out" });
	}

	
	[HttpGet("me")]
	[Authorize]
	public async Task<IActionResult> GetCurrentUser()
	{

		var model = await userService.GetUserProfileAsync();
		return Ok(model);
	}
	[HttpPost("forgot-password")]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
	{
		bool res = await userService.ForgotPasswordAsync(model);
		if (res)
			return Ok();
		else
			return BadRequest(new
			{
				Status = 400,
				IsValid = false,
				Errors = new { Email = "Користувача з такою поштою не існує" }
			});
	}
	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
	{
		var isTry = await userService.ResetPasswordAsync(model);
		if (!isTry)
		{
			return BadRequest(new
			{
				Status = 400,
				IsValid = false,
				Errors = new { Email = "Невірні дані для відновлення паролю" }
			});
		}
		return Ok();
	}
	[HttpGet]
	public async Task<IActionResult> Search([FromQuery] UserSearchModel model)
	{
		//Обчислення часу виконання
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		var result = await userService.SearchAsync(model);
		stopwatch.Stop();
		// Get the elapsed time as a TimeSpan value.
		TimeSpan ts = stopwatch.Elapsed;
		// Format and display the TimeSpan value.
		string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			ts.Hours, ts.Minutes, ts.Seconds,
			ts.Milliseconds / 10);
		Console.WriteLine("-----------Elapsed Time------------: " + elapsedTime);
		return Ok(result);
	}
}

public class GoogleLogin
{
	public string Token { get; set; } = null!;
}