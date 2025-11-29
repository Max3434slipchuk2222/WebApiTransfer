using Microsoft.AspNetCore.Http;

namespace Core.Models.Location.Country;

public class CountryEditModel
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Code { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public IFormFile? Image { get; set; } 
}