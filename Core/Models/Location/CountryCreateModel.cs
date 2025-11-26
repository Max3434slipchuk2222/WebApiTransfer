using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Location;

public class CountryCreateModel
{
	public string Name { get; set; } = null!;
	public string Code { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public IFormFile Image { get; set; } = null!;
}
