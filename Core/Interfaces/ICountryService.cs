using Core.Models.Location.Country;

namespace Core.Interfaces;

public interface ICountryService
{
	Task<List<CountryItemModel>> GetListAsync();
	Task<CountryItemModel> CreateAsync(CountryCreateModel model);
	Task UpdateAsync(CountryEditModel model);
	Task DeleteAsync(int id);
}
