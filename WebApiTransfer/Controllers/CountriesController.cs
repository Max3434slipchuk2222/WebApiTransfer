using Core.Interfaces;
using Core.Models.Location;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApiTransfer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController(ICountryService countryService) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetCountries()
		{
			var list = await countryService.GetListAsync();

			return Ok(list);
		}
		[HttpPost]
		public async Task<IActionResult> CreateCountry([FromForm] CountryCreateModel model)
		{
			var item = await countryService.CreateAsync(model);
			return CreatedAtAction(null, item);
		}
	}
}
