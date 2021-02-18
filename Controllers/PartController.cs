namespace Sila.Controllers
{
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;

	/// <summary>
	/// The controller for interacting with parts.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class PartController : ControllerBase
	{

		public PartController()
		{

		}

		[HttpPost]
		public async Task<IActionResult> CreatePart()
		{
			return BadRequest("Not Implemented");
		}

		[HttpDelete]
		public async Task<IActionResult> DeletePart()
		{
			return BadRequest("Not Implemented");
		}

		[HttpGet]
		public async Task<IActionResult> GetPart()
		{
			return Ok("Ayyo!");
		}
	}
}
