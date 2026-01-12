using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTransfer.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TransportationController(ITransportationService ts) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> GetListAsync()
		{
			var result = await ts.GetListAsync();
			return Ok(result);
		}
	}
}
