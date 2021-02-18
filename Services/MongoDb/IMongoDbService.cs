namespace Sila.Services.MongoDb
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using MongoDB.Driver.Linq;
	using Sila.Models.Database;

	/// <summary>
	/// A service for interacting with MongoDb.
	/// </summary>
	public interface IMongoDbService
	{
		/// <summary>
		/// Get a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <typeparam name="T">The type of documents stored in the collection.</typeparam>
		/// <returns>A queryable collection.</returns>
		IMongoQueryable<T> GetCollection<T>(String collection) where T : IDatabaseObject;

		/// <summary>
		/// Insert a document into a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="document">The document to insert.</param>
		/// <typeparam name="T">The type of the document.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task InsertDocumentAsync<T>(String collection, T document) where T : IDatabaseObject;

		/// <summary>
		/// Insert documents into a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="documents">The documents to insert.</param>
		/// <typeparam name="T">The type of the documents.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task InsertDocumentsAsync<T>(String collection, IEnumerable<T> documents) where T : IDatabaseObject;

		/// <summary>
		/// Add an item to a set on a given path.
		/// Dot notation path example: prop1.0.nestedProp
		/// {
		/// 	prop1: [
		/// 		{
		/// 			nestedProp: [
		/// 							...
		/// 						]
		/// 		},
		/// 		...
		/// 	]
		/// }
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root item to update.</param>
		/// <param name="dotNotationPath">The dot notation path. See example in summary.</param>
		/// <param name="item">The item to add to the set.</param>
		/// <typeparam name="TDocument">The type of the root item.</typeparam>
		/// <typeparam name="TItem">The type of the item to add to the set.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task AddItemToSet<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String dotNotationPath, TItem item) where TDocument : IDatabaseObject;

		/// <summary>
		/// Add a property to an object.
		/// Example 1: dotNotationPath = null, propertyName = "newProp", item = "newValue"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp": { ... },
		/// 	"newProp": "newValue"
		/// }
		/// Example 2: dotNotationPath = "OtherOldProp", propertyName = "newProp", item = "newValue"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp":
		/// 		{
		/// 			"newProp": "newValue",
		/// 			...
		/// 		}
		/// }
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root item to update.</param>
		/// <param name="dotNotationPath">The dot notation path. Null indicates to add it to the root item. See example in summary.</param>
		/// <param name="propertyName">The name of the property to add.</param>
		/// <param name="item">The item to add as the value of the property.</param>
		/// <typeparam name="TDocument">The type of the root item.</typeparam>
		/// <typeparam name="TItem">The type of the item to add as a property.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task AddPropertyToObject<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName, TItem item) where TDocument : IDatabaseObject;

		/// <summary>
		/// Add a property to objects matching the filter.
		/// Example 1: dotNotationPath = null, propertyName = "newProp", item = "newValue"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp": { ... },
		/// 	"newProp": "newValue"
		/// }
		/// Example 2: dotNotationPath = "OtherOldProp", propertyName = "newProp", item = "newValue"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp":
		/// 		{
		/// 			"newProp": "newValue",
		/// 			...
		/// 		}
		/// }
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root items to update.</param>
		/// <param name="dotNotationPath">The dot notation path. Null indicates to add it to the root items. See example in summary.</param>
		/// <param name="propertyName">The name of the property to add.</param>
		/// <param name="item">The item to add as the value of the property.</param>
		/// <typeparam name="TDocument">The type of the root items.</typeparam>
		/// <typeparam name="TItem">The type of the item to add as a property.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task AddPropertyToObjects<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName, TItem item) where TDocument : IDatabaseObject;

		/// <summary>
		/// Add properties to an object. See 'AddPropertyToObject' for an example as this behaves the same way.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root item to update.</param>
		/// <param name="updateDefinitions">The list of update definitions.</param>
		/// <typeparam name="TDocument">The type of the root item.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task AddPropertiesToObject<TDocument>(String collection, Expression<Func<TDocument, Boolean>> filter, IEnumerable<MongoDb.StackableUpdateDefinition> updateDefinitions) where TDocument : IDatabaseObject;

		/// <summary>
		/// Remove a property from an object.
		/// Example 1: dotNotationPath = null, propertyName = "newProp"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp": { ... },
		/// 	"newProp": "newValue" // This property would be removed
		/// }
		/// Example 2: dotNotationPath = "OtherOldProp", propertyName = "newProp"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp":
		/// 		{
		/// 			"newProp": "newValue", // This property would be removed
		/// 			...
		/// 		}
		/// }
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root item to update.</param>
		/// <param name="dotNotationPath">The dot notation path. Null indicates to remove it to the root item. See example in summary.</param>
		/// <param name="propertyName">The name of the property to remove.</param>
		/// <typeparam name="TDocument">The type of the root item.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task RemovePropertyFromObject<TDocument>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName) where TDocument : IDatabaseObject;

		/// <summary>
		/// Remove a document from a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">A filter for the document to remove.</param>
		/// <typeparam name="T">The type of the document to remove.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task RemoveDocumentAsync<T>(String collection, Expression<Func<T, Boolean>> filter) where T : IDatabaseObject;

		/// <summary>
		/// Remove documents from a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">A filter for the documents to remove from the collection.</param>
		/// <typeparam name="T">The type of documents to remove from the collection.</typeparam>
		/// <returns>A task representing the operation.</returns>
		Task RemoveDocumentsAsync<T>(String collection, Expression<Func<T, Boolean>> filter) where T : IDatabaseObject;

		/// <summary>
		/// Find the first index of an element matching a predicate in the provided mongo queryable.
		/// </summary>
		/// <param name="mongoQueryable">The mongo queryable.</param>
		/// <param name="predicate">The predicate.</param>
		/// <typeparam name="T">The type of document.</typeparam>
		/// <returns>The index of the element of null otherwise.</returns>
		Task<Int32?> GetIndex<T>(IMongoQueryable<T> mongoQueryable, Func<T, Boolean> predicate) where T : IDatabaseObject;
	}
}