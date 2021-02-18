namespace Sila.Models.Database
{
	using System;

	/// <summary>
	/// Describes the database model for an assembly or a part.
	/// </summary>
	public abstract class AssemblyOrPart : IDatabaseObject
	{
		/// <summary>
		/// The database identifier.
		/// </summary>
		public String Id { get; }

		/// <summary>
		/// The name of the part or assembly.
		/// </summary>
		public String Name { get; }

		/// <summary>
		/// The parent assembly of this part or assembly if it belongs to an assembly.
		/// </summary>
		public String? ParentAssemblyId { get; set; }

		/// <summary>
		/// Create a new assembly or part.
		/// </summary>
		/// <param name="id">The database identifier.</param>
		/// <param name="name">The name of the part or assembly.</param>
		public AssemblyOrPart(String id, String name)
		{
			Id = id;
			Name = name;
		}
	}
}