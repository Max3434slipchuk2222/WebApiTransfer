using Core.Models.Location;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApiTransfer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountriesController : ControllerBase
	{
		private readonly AppDbTransferContext context;
		public CountriesController(AppDbTransferContext context)
		{
			this.context = context;
		}
		[HttpGet]
		public async Task<IActionResult> GetCountries()
		{
			var countries = await context.Countries
				.Select(x => new CountryItemModel
				{
					Id = x.Id,
					Name = x.Name,
					Code = x.Code,
					Slug = x.Slug,
					Image = x.Image
				})
				.ToListAsync();

			return Ok(countries);
		}
	}
}
