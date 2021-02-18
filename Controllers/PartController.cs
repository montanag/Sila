namespace Sila.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using Sila.Models.Database;
	using Sila.Models.Rest.Requests;
	using Sila.Services.MongoDb;

	/// <summary>
	/// The controller for interacting with parts.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class PartController : ControllerBase
	{
		private readonly IMongoDbService _mongoDbService;

		/// <summary>
		/// The controller for interacting with parts.
		/// </summary>
		/// <param name="mongoDbService">The MongoDb service to use.</param>
		public PartController(IMongoDbService mongoDbService)
		{
			_mongoDbService = mongoDbService;
		}

		/// <summary>
		/// Create a new part
		/// </summary>
		/// <param name="createPartRequest">The create part request.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpPost]
		public async Task<IActionResult> CreatePart([FromBody] CreatePartRequest createPartRequest)
		{
			// Create the new part
			Part part = new(ObjectId.GenerateNewId().ToString(), createPartRequest.Name)
			{
				Color = createPartRequest.Color,
				Material = createPartRequest.Material
			};

			// Add the part to the database
			await _mongoDbService.InsertDocumentAsync(nameof(Part), part).ConfigureAwait(false);

			// Return the created part
			return Ok(part);
		}

		/// <summary>
		/// Delete a part.
		/// </summary>
		/// <param name="id">The ID of the part to delete.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePart(String id)
		{
			// Delete the part with the given ID
			await _mongoDbService.RemoveDocumentAsync<Part>(nameof(Part), part => part.Id == id).ConfigureAwait(false);

			// Return no content
			return NoContent();
		}

		/// <summary>
		/// Get a part.
		/// </summary>
		/// <param name="id">The ID of the part.</param>
		/// <returns>A task containing an HTTP response with the part.</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPart(String id)
		{
			// Find the part with the given ID
			Part part = await _mongoDbService.GetCollection<Part>(nameof(Part)).FirstOrDefaultAsync(part => part.Id == id);

			// Return the part
			return Ok(part);
		}

		/// <summary>
		/// Get all parts.
		/// </summary>
		/// <returns>A task containing an HTTP response with the parts.</returns>
		[HttpGet]
		public async Task<IActionResult> GetParts()
		{
			// Get the parts as a list
			List<Part> parts = await _mongoDbService.GetCollection<Part>(nameof(Part)).ToListAsync();

			// Return the parts
			return Ok(parts);
		}
	}
}
