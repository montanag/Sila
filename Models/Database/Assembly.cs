namespace Sila.Models.Database
{
	using System;

	/// <summary>
	/// Describes a database model for an assembly.
	/// </summary>
	public sealed class Assembly : AssemblyOrPart
	{
		/// <summary>
		/// Create a new assembly.
		/// </summary>
		/// <param name="id">The database identifier.</param>
		/// <param name="name">The name of the part or assembly.</param>
		public Assembly(String id, String name) : base(id, name)
		{
		}
	}
}