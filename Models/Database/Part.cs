namespace Sila.Models.Database
{
	using System;

	/// <summary>
	/// Describes a database model for a part.
	/// </summary>
	public class Part : AssemblyOrPart
	{
		/// <summary>
		/// The color of the part.
		/// </summary>
		public String? Color { get; set; }

		/// <summary>
		/// The material of the part.
		/// </summary>
		public String? Material { get; set; }

		/// <summary>
		/// Create a new part.
		/// </summary>
		/// <param name="id">The database identifier.</param>
		/// <param name="name">The name of the part or assembly.</param>
		public Part(String id, String name) : base(id, name)
		{
		}
	}
}