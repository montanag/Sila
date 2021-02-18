namespace Sila.Models.Rest.Requests
{
	using System;

	/// <summary>
	/// The structure of a create assembly request.
	/// </summary>
	public class CreateAssemblyRequest
	{
		/// <summary>
		/// The name of the assembly.
		/// </summary>
		public String Name { get; }

		/// <summary>
		/// Instantiate a create assembly request.
		/// </summary>
		/// <param name="name">The name of the assembly.</param>
		public CreateAssemblyRequest(String name)
		{
			Name = name;
		}
	}
}