namespace Sila.Services.MongoDb
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using MongoDB.Driver;
	using MongoDB.Driver.Linq;
	using Sila.Models.Database;
	using Sila.Options;

	/// <summary>
	/// The service for interacting with MongoDb.
	/// </summary>
	public class MongoDbService : IMongoDbService
	{
		private readonly IMongoDatabase _database;

		/// <summary>
		/// The service for interacting with MongoDb.
		/// </summary>
		/// <param name="mongoDbOptions">The MongoDb options to use.</param>
		public MongoDbService(MongoDbOptions mongoDbOptions)
		{
			if (mongoDbOptions == null) throw new ArgumentNullException(nameof(mongoDbOptions));

			MongoClient mongoClient = new MongoClient(mongoDbOptions.ConnectionString);
			_database = mongoClient.GetDatabase(mongoDbOptions.Database);
		}

		/// <summary>
		/// Get a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <typeparam name="T">The type of documents stored in the collection.</typeparam>
		/// <returns>A queryable collection.</returns>
		public IMongoQueryable<T> GetCollection<T>(String collection) where T : IDatabaseObject
		{
			return _database.GetCollection<T>(collection).AsQueryable();
		}

		/// <summary>
		/// Insert a document into a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="document">The document to insert.</param>
		/// <typeparam name="T">The type of the document.</typeparam>
		/// <returns>A task representing the operation.</returns>
		public Task InsertDocumentAsync<T>(String collection, T document) where T : IDatabaseObject
		{
			return _database.GetCollection<T>(collection).InsertOneAsync(document);
		}

		/// <summary>
		/// Insert documents into a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="documents">The documents to insert.</param>
		/// <typeparam name="T">The type of the documents.</typeparam>
		/// <returns>A task representing the operation.</returns>
		public Task InsertDocumentsAsync<T>(String collection, IEnumerable<T> documents) where T : IDatabaseObject
		{
			if (documents == null) throw new ArgumentNullException(nameof(documents));

			return _database.GetCollection<T>(collection).InsertManyAsync(documents);
		}

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
		public Task AddItemToSet<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String dotNotationPath, TItem item) where TDocument : IDatabaseObject
		{
			UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.AddToSet(dotNotationPath, item);
			return _database.GetCollection<TDocument>(collection).UpdateOneAsync(filter, updateDefinition);
		}

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
		public Task AddPropertyToObject<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName, TItem item) where TDocument : IDatabaseObject
		{
			// Check if we need to add a '.' in the middle for nested objects.
			String fullDotNotationPath = dotNotationPath != null ? $"{dotNotationPath}.{propertyName}" : propertyName;

			UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Set(fullDotNotationPath, item);
			return _database.GetCollection<TDocument>(collection).UpdateOneAsync(filter, updateDefinition);
		}

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
		public Task AddPropertyToObjects<TDocument, TItem>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName, TItem item) where TDocument : IDatabaseObject
		{
			// Check if we need to add a '.' in the middle for nested objects.
			String fullDotNotationPath = dotNotationPath != null ? $"{dotNotationPath}.{propertyName}" : propertyName;

			UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Set(fullDotNotationPath, item);
			return _database.GetCollection<TDocument>(collection).UpdateManyAsync(filter, updateDefinition);
		}

		/// <summary>
		/// Add properties to an object. See 'AddPropertyToObject' for an example as this behaves the same way.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">The filter for the root item to update.</param>
		/// <param name="updateDefinitions">The list of update definitions.</param>
		/// <typeparam name="TDocument">The type of the root item.</typeparam>
		/// <returns>A task representing the operation.</returns>
		public Task AddPropertiesToObject<TDocument>(String collection, Expression<Func<TDocument, Boolean>> filter, IEnumerable<StackableUpdateDefinition> updateDefinitions) where TDocument : IDatabaseObject
		{
			// Ensure the update definitions contains items
			if (updateDefinitions == null || !updateDefinitions.Any()) { return Task.CompletedTask; }

			UpdateDefinition<TDocument>? stackedUpdateDefinitions = null;
			foreach (StackableUpdateDefinition updateDefinition in updateDefinitions)
			{
				// Check if we need to add a '.' in the middle for nested objects.
				String fullDotNotationPath = updateDefinition.DotNotationPath != null ? $"{updateDefinition.DotNotationPath}.{updateDefinition.PropertyName}" : updateDefinition.PropertyName;

				// Create or add to the current update definition chain
				stackedUpdateDefinitions = stackedUpdateDefinitions?.Set(fullDotNotationPath, updateDefinition.Item) ?? Builders<TDocument>.Update.Set(fullDotNotationPath, updateDefinition.Item);
			}

			return _database.GetCollection<TDocument>(collection).UpdateOneAsync(filter, stackedUpdateDefinitions);
		}

		/// <summary>
		/// Remove a property from an object.
		/// Example 1: dotNotationPath = null, propertyName = "newProp"
		/// {
		/// 	"oldProp": ...,
		/// 	"OtherOldProp": { ... },
		/// /// 	"newProp": "newValue" // This property would be removed
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
		public Task RemovePropertyFromObject<TDocument>(String collection, Expression<Func<TDocument, Boolean>> filter, String? dotNotationPath, String propertyName) where TDocument : IDatabaseObject
		{
			// Check if we need to add a '.' in the middle for nested objects.
			String fullDotNotationPath = dotNotationPath != null ? $"{dotNotationPath}.{propertyName}" : propertyName;

			UpdateDefinition<TDocument> updateDefinition = Builders<TDocument>.Update.Unset(fullDotNotationPath);
			return _database.GetCollection<TDocument>(collection).UpdateOneAsync(filter, updateDefinition);
		}

		/// <summary>
		/// Remove a document from a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">A filter for the document to remove.</param>
		/// <typeparam name="T">The type of the document to remove.</typeparam>
		/// <returns>A task representing the operation.</returns>
		public Task RemoveDocumentAsync<T>(String collection, Expression<Func<T, Boolean>> filter) where T : IDatabaseObject
		{
			return _database.GetCollection<T>(collection).DeleteOneAsync(filter);
		}

		/// <summary>
		/// Remove documents from a collection.
		/// </summary>
		/// <param name="collection">The name of the collection.</param>
		/// <param name="filter">A filter for the documents to remove from the collection.</param>
		/// <typeparam name="T">The type of documents to remove from the collection.</typeparam>
		/// <returns>A task representing the operation.</returns>
		public Task RemoveDocumentsAsync<T>(String collection, Expression<Func<T, Boolean>> filter) where T : IDatabaseObject
		{
			return _database.GetCollection<T>(collection).DeleteManyAsync(filter);
		}

		/// <summary>
		/// Find the first index of an element matching a predicate in the provided mongo queryable.
		/// </summary>
		/// <param name="mongoQueryable">The mongo queryable.</param>
		/// <param name="predicate">The predicate.</param>
		/// <typeparam name="T">The type of document.</typeparam>
		/// <returns>The index of the element of null otherwise.</returns>
		public async Task<Int32?> GetIndex<T>(IMongoQueryable<T> mongoQueryable, Func<T, Boolean> predicate) where T : IDatabaseObject
		{
			// Ensure the arguments are not null
			if (mongoQueryable == null) throw new ArgumentNullException(nameof(mongoQueryable));

			// Get the cursor
			IAsyncCursor<T> cursor = await mongoQueryable.ToCursorAsync().ConfigureAwait(false);

			// Move the first batch onto the cursor
			Boolean documentsAvailable = await cursor.MoveNextAsync().ConfigureAwait(false);

			Int32 index = 0;
			while (documentsAvailable)
			{
				foreach (T item in cursor.Current)
				{
					if (predicate(item)) return index;
					index++;
				}
				documentsAvailable = await cursor.MoveNextAsync().ConfigureAwait(false);
			}

			return null;
		}
	}
}