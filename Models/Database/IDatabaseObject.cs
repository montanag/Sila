namespace Sila.Models.Database {
	using System;

	/// <summary>
	/// Represents an object able to be stored in a database.
	/// </summary>
	public interface IDatabaseObject
	{
		/// <summary>
		/// The object identifier.
		/// </summary>
		String Id { get; }
	}
}