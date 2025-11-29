using AutoMapper;
using Core.Models.Location.Country;
using Domain.Entities.Location;

namespace Core.Mappers;

public class CountryMapper : Profile
{
	public CountryMapper()
	{
		CreateMap<CountryEntity, CountryItemModel>();
		CreateMap<CountryCreateModel, CountryEntity>()
			.ForMember(x => x.Image, opt => opt.Ignore());
		CreateMap<CountryEditModel, CountryEntity>()
			.ForMember(x => x.Image, opt => opt.Ignore()) 
			.ForMember(x => x.DateCreated, opt => opt.Ignore()) 
			.ForMember(x => x.IsDeleted, opt => opt.Ignore());

	}
}
