namespace Sila.Options
{
	using System;
	using Microsoft.Extensions.Configuration;

	/// <summary>
	/// Configuration options for Mongo db.
	/// </summary>
	public class MongoDbOptions
	{
		/// <summary>
		/// The connection string to use.
		/// </summary>
		public String ConnectionString { get; }

		/// <summary>
		/// The database to use.
		/// </summary>
		public String Database { get; }

		/// <summary>
		/// Configuration options for Mongo db.
		/// </summary>
		/// <param name="connectionString">The connection string to use.</param>
		/// <param name="database">The database to use.</param>
		public MongoDbOptions(String connectionString, String database)
		{
			ConnectionString = connectionString;
			Database = database;
		}

		/// <summary>
		/// Create a MongoDbOptions instance from a configuration section.
		/// </summary>
		/// <param name="mongoDbConfiguration">The configuration section.</param>
		/// <returns>A validated MongoDbOptions instance.</returns>
		internal static MongoDbOptions FromConfiguration(IConfiguration mongoDbConfiguration)
		{
			// Validate the connection string
			String? connectionString = mongoDbConfiguration[nameof(ConnectionString)];
			if (connectionString == null)
			{
				throw new ArgumentException(nameof(ConnectionString));
			}

			// Validate the database
			String? database = mongoDbConfiguration[nameof(Database)];
			if (database == null)
			{
				throw new ArgumentException(nameof(Database));
			}

			return new MongoDbOptions(connectionString, database);
		}
	}
}