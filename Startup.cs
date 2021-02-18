namespace Sila
{
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Sila.Options;
	using Sila.Services.MongoDb;

	/// <summary>
	/// Gets called by the entry point to initialize the web server.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Gets called by the entry point to initialize the web server.
		/// </summary>
		/// <param name="configuration">The configuration to use.</param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		/// <summary>
		/// The configuration for the web server.
		/// </summary>
		public IConfiguration Configuration { get; }

		/// <summary>
		/// This method gets called by the runtime. Use this method to add services to the container.
		/// </summary>
		/// <param name="services">The service collection to add services to.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			// Add controllers
			services.AddControllers();

			// Configure the MongoDb options
			IConfiguration mongoDbConfiguration = Configuration.GetSection("MongoDb");
			MongoDbOptions mongoDbOptions;
			try
			{
				mongoDbOptions = MongoDbOptions.FromConfiguration(mongoDbConfiguration);
			}
			catch (Exception exception)
			{
				throw new Exception($"Error configuring {nameof(MongoDbOptions)}. {exception.Message}");
			}

			// Add services
			services.AddTransient<IMongoDbService, MongoDbService>();
			services.AddSingleton(mongoDbOptions);
		}

		/// <summary>
		/// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		/// </summary>
		/// <param name="app">The application builder to use.</param>
		/// <param name="env">The environment to use.</param>
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			// TODO: Add authorization and authentication
			// app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
