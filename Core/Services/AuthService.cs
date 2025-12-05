using Core.Interfaces;
using Domain.Entities.Idenity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
	public class AuthService(IHttpContextAccessor httpContextAccessor, UserManager<UserEntity> userManager) : IAuthService
	{
		public async Task<int> GetUserIdAsync()
		{
			var email = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
			if(string.IsNullOrEmpty(email))
			{
				throw new UnauthorizedAccessException("User is not authenticated");
			}
			var user = await userManager.FindByEmailAsync(email);
			return user!.Id;

		}
	}
}
