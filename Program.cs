namespace Sila
{
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Hosting;

	/// <summary>
	/// Contains the entry point to the application.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The entry point to the application.
		/// </summary>
		/// <param name="args">The arguments passed to the application.</param>
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		/// <summary>
		/// Create the host builder.
		/// </summary>
		/// <param name="args">The arguments passed to the application.</param>
		/// <returns>The host builder.</returns>
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
