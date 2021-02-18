namespace Sila.Models.Rest.Requests
{
	using System;

	/// <summary>
	/// The structure of a create part request.
	/// </summary>
	public class CreatePartRequest
	{
		/// <summary>
		/// The name of the part.
		/// </summary>
		public String Name { get; set; }

		/// <summary>
		/// The color of the part.
		/// </summary>
		public String? Color { get; set; }

		/// <summary>
		/// The material of the part.
		/// </summary>
		public String? Material { get; set; }
	}
}