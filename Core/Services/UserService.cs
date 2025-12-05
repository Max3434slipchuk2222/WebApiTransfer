using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interfaces;
using Core.Models.Account;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
	public class UserService(IAuthService authService,IMapper mapper, AppDbTransferContext appDbTransferContext) : IUserService
	{
		public async Task<UserProfileModel> GetUserProfileAsync()
		{
			var userId = await authService.GetUserIdAsync();

			var profile = await appDbTransferContext.Users
				.ProjectTo<UserProfileModel>(mapper.ConfigurationProvider)
				.SingleOrDefaultAsync(u => u.Id == userId!);

			return profile!;
		}
	}
}
