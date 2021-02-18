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
		public async Task<IActionResult> GetParts([FromQuery] Boolean componentPartsOnly, [FromQuery] Boolean orphanPartsOnly)
		{
			// Get the parts as a mongo queryable
			IMongoQueryable<Part>? parts = _mongoDbService.GetCollection<Part>(nameof(Part));

			if (componentPartsOnly && orphanPartsOnly)
			{
				return BadRequest($"Please specify true for only one of {nameof(componentPartsOnly)} or {nameof(orphanPartsOnly)}.");
			}
			else if (componentPartsOnly)
			{
				return Ok(await parts.Where(part => part.ParentAssemblyId != null).ToListAsync().ConfigureAwait(false));
			}
			else if (orphanPartsOnly)
			{
				return Ok(await parts.Where(part => part.ParentAssemblyId == null).ToListAsync().ConfigureAwait(false));
			}
			else
			{
				return Ok(await parts.ToListAsync().ConfigureAwait(false));
			}
		}

		/// <summary>
		/// Get all the parent assemblies for a part.
		/// </summary>
		/// <param name="id">The ID of the part.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpGet("{id}/parent")]
		public async Task<IActionResult> GetParentAssemblies(String id)
		{
			// Get the first parent ID
			String? parentId = await _mongoDbService.GetCollection<Part>(nameof(Part)).Where(part => part.Id == id).Select(part => part.ParentAssemblyId).FirstOrDefaultAsync().ConfigureAwait(false);

			// Instantiate the list of parents
			List<String> parents = new();

			// While the current parent is not null, look for the next parent
			while (parentId != null)
			{
				parents.Add(parentId);

				parentId = await _mongoDbService.GetCollection<Assembly>(nameof(Assembly)).Where(assembly => assembly.Id == parentId).Select(assembly => assembly.ParentAssemblyId).FirstOrDefaultAsync().ConfigureAwait(false);
			}

			// Return the parents
			return Ok(parents);
		}

		/// <summary>
		/// Update the parent of a part.
		/// </summary>
		/// <param name="id">The ID of a part to update.</param>
		/// <param name="updateParentRequest">The update parent request.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateParent(String id, [FromBody] UpdateParentRequest updateParentRequest)
		{
			// Add the parent ID to the part
			await _mongoDbService.AddPropertyToObject<Assembly, String?>(nameof(Part), part => part.Id == id, null, nameof(Part.ParentAssemblyId), updateParentRequest.ParentAssemblyId);

			// Return an OK response
			return Ok();
		}
	}
}
