using AutoMapper;
using Domain.Entities.Idenity;
using Core.Models.Account;
namespace Core.Mappers
{
	public class UserMapper : Profile
	{
		public UserMapper()
		{
			CreateMap<UserEntity, UserProfileModel>()
				.ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
				.ForMember(x => x.Phone, opt => opt.MapFrom(x => $"{x.PhoneNumber}"));
			CreateMap<UserEntity, UserItemModel>()
				.ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.LastName} {x.FirstName}"))
				.ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles!.Select(ur => ur.Role.Name).ToList()));
				//.ForMember(x => x.Phone, opt => opt.MapFrom(x => x.PhoneNumber));
		}
	}
}
