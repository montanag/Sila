namespace Sila.Services.MongoDb
{
	using System;

	/// <summary>
	/// An update definition that can be stacked to perform many updates in a single query.
	/// </summary>
	public class StackableUpdateDefinition
	{
		/// <summary>
		/// The dot notation path to the object to update. To update a property on the root object this would be left null; use it for nested properties.
		/// </summary>
		public String? DotNotationPath { get; }

		/// <summary>
		/// The property name to update.
		/// </summary>
		public String PropertyName { get; }

		/// <summary>
		/// The item to set for the property.
		/// </summary>
		public Object? Item { get; }

		/// <summary>
		/// An update definition that can be stacked to perform many updates in a single query.
		/// </summary>
		/// <param name="dotNotationPath">The dot notation path to the object to update. To update a property on the root object this would be left null; use it for nested properties.</param>
		/// <param name="propertyName">The property name to update.</param>
		/// <param name="item">The item to set for the property.</param>
		public StackableUpdateDefinition(String? dotNotationPath, String propertyName, Object? item)
		{
			DotNotationPath = dotNotationPath;
			PropertyName = propertyName;
			Item = item;
		}
	}
}