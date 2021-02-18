namespace Sila.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using MongoDB.Bson;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using Sila.Models.Database;
	using Sila.Models.Rest.Requests;
	using Sila.Services.MongoDb;

	/// <summary>
	/// The controller for interacting with assemblies.
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class AssemblyController : ControllerBase
	{
		private readonly IMongoDbService _mongoDbService;

		/// <summary>
		/// The controller for interacting with assemblies.
		/// </summary>
		/// <param name="mongoDbService">The MongoDb service to use.</param>
		public AssemblyController(IMongoDbService mongoDbService)
		{
			_mongoDbService = mongoDbService;
		}

		/// <summary>
		/// Create a new assembly
		/// </summary>
		/// <param name="createAssemblyRequest">The create assembly request.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpPost]
		public async Task<IActionResult> CreateAssembly([FromBody] CreateAssemblyRequest createAssemblyRequest)
		{
			// Create the new assembly
			Assembly assembly = new(ObjectId.GenerateNewId().ToString(), createAssemblyRequest.Name)
			{
				// Insert optional assembly properties as added
			};

			// Add the assembly to the database
			await _mongoDbService.InsertDocumentAsync(nameof(Assembly), assembly).ConfigureAwait(false);

			// Return the created assembly
			return Ok(assembly);
		}

		/// <summary>
		/// Delete a assembly.
		/// </summary>
		/// <param name="id">The ID of the assembly to delete.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAssembly(String id)
		{
			// Find any children of this assembly and set their parent to null
			await _mongoDbService.AddPropertyToObjects<Part, String?>(nameof(Part), part => part.ParentAssemblyId == id, null, nameof(Part.ParentAssemblyId), null);
			await _mongoDbService.AddPropertyToObjects<Assembly, String?>(nameof(Assembly), assembly => assembly.ParentAssemblyId == id, null, nameof(Assembly.ParentAssemblyId), null);

			// Delete the assembly with the given ID
			await _mongoDbService.RemoveDocumentAsync<Assembly>(nameof(Assembly), assembly => assembly.Id == id).ConfigureAwait(false);

			// Return no content
			return NoContent();
		}

		/// <summary>
		/// Get a assembly.
		/// </summary>
		/// <param name="id">The ID of the assembly.</param>
		/// <returns>A task containing an HTTP response with the assembly.</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetAssembly(String id)
		{
			// Find the assembly with the given ID
			Assembly assembly = await _mongoDbService.GetCollection<Assembly>(nameof(Assembly)).FirstOrDefaultAsync(assembly => assembly.Id == id);

			// Return the assembly
			return Ok(assembly);
		}

		/// <summary>
		/// Get all assemblies.
		/// </summary>
		/// <returns>A task containing an HTTP response with the assemblies.</returns>
		[HttpGet]
		public async Task<IActionResult> GetAssemblies([FromQuery] Boolean topLevelOnly, [FromQuery] Boolean subAssembliesOnly)
		{
			// Get the assemblies as a mongo queryably
			IMongoQueryable<Assembly> assemblies = _mongoDbService.GetCollection<Assembly>(nameof(Assembly));

			if (topLevelOnly && subAssembliesOnly)
			{
				return BadRequest($"Please specify true for only one of {nameof(topLevelOnly)} or {nameof(subAssembliesOnly)}.");
			}
			else if (topLevelOnly)
			{
				return Ok(await assemblies.Where(assembly => assembly.ParentAssemblyId == null).ToListAsync().ConfigureAwait(false));
			}
			else if (subAssembliesOnly)
			{
				return Ok(await assemblies.Where(assembly => assembly.ParentAssemblyId != null).ToListAsync().ConfigureAwait(false));
			}
			else
			{
				return Ok(await assemblies.ToListAsync().ConfigureAwait(false));
			}
		}

		/// <summary>
		/// Get the children of an assembly.
		/// </summary>
		/// <param name="id">The ID of the assembly to get children for.</param>
		/// <param name="firstLevelOnly">Whether or not to only include the first level children.</param>
		/// <param name="componentPartsOnly">Whether or not to include only the component parts.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpGet("{id}/children")]
		public async Task<IActionResult> GetChildren(String id, [FromQuery] Boolean firstLevelOnly, [FromQuery] Boolean componentPartsOnly)
		{
			// TODO: Optionally construct the json child/parent tree

			// Create the flattened list of children to return
			List<AssemblyOrPart> children = new();

			// Add the children parts
			children.AddRange(await _mongoDbService.GetCollection<Part>(nameof(Part)).Where(part => part.ParentAssemblyId == id).ToListAsync().ConfigureAwait(false));

			if (firstLevelOnly)
			{
				// Add the children assemblies
				children.AddRange(await _mongoDbService.GetCollection<Assembly>(nameof(Assembly)).Where(assembly => assembly.ParentAssemblyId == id).ToListAsync().ConfigureAwait(false));

				return Ok(children);
			}

			// Get the child assemblies
			List<Assembly> childAssemblies = await _mongoDbService.GetCollection<Assembly>(nameof(Assembly)).Where(assembly => assembly.ParentAssemblyId == id).ToListAsync().ConfigureAwait(false);

			while (childAssemblies.Count > 0)
			{
				// Get the next child assembly
				Assembly childAssembly = childAssemblies.First();

				// Conditionally add the child assembly to the children to return
				if (!componentPartsOnly)
				{
					children.Add(childAssembly);
				}

				// Remove the child assembly from the list of child assemblies to iterate over
				childAssemblies.Remove(childAssembly);

				// Add any grand child assemblies to the list to iterate over
				childAssemblies.AddRange(await _mongoDbService.GetCollection<Assembly>(nameof(Assembly)).Where(assembly => assembly.ParentAssemblyId == childAssembly.Id).ToListAsync().ConfigureAwait(false));

				// Add any component parts to the list of children to return
				children.AddRange(await _mongoDbService.GetCollection<Part>(nameof(Part)).Where(part => part.ParentAssemblyId == childAssembly.Id).ToListAsync().ConfigureAwait(false));
			}

			// Return the children
			return Ok(children);
		}

		/// <summary>
		/// Update the parent of an assembly.
		/// </summary>
		/// <param name="id">The ID of an assembly to update.</param>
		/// <param name="updateParentRequest">The update parent request.</param>
		/// <returns>A task containing an HTTP response.</returns>
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateParent(String id, [FromBody] UpdateParentRequest updateParentRequest)
		{
			// Add the parent ID to the assembly
			await _mongoDbService.AddPropertyToObject<Assembly, String?>(nameof(Assembly), assembly => assembly.Id == id, null, nameof(Assembly.ParentAssemblyId), updateParentRequest.ParentAssemblyId);

			// Return an OK response
			return Ok();
		}
	}
}
