namespace Sila.Models.Rest.Requests
{
	using System;

	/// <summary>
	/// The structure of an update parent request.
	/// </summary>
	public class UpdateParentRequest
	{
		/// <summary>
		/// The new parent ID.
		/// </summary>
		public String? ParentAssemblyId { get; set; }
	}
}