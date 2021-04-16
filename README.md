# HelpersForMicroservices
Common libraries for microservices written on .net core

### Infrastructure.Api ###
#### Program.cs
For checking if DB has all latest migrations during service start, if DB is not up to date, apply all pending migrations and execute data seeder.

    CreateHostBuilder(args).Build()
		.MigrateDbContext<DataPrivacyDbContext>((context, services) =>
		{
			var seedLogger = services.GetRequiredService<ILogger<DataPrivacyDbContext>>();
			new DataPrivacyDbContextSeed()
				.SeedAsync(context, seedLogger)
				.Wait();
		}).Run();

Where is:
- DataPrivacyDbContext - your DbContext
- DataPrivacyDbContextSeed - your DB seed class

#### Startup.cs
For registering global exception filter and request validator

    services.AddControllers(options =>
	{
		options.Filters.Add(typeof(HttpGlobalExceptionFilter));
		options.Filters.Add(typeof(ValidateModelStateFilter));
	});

For registering API logger(to log all requests and responses)

    // Should be registered before UseEndpoints
    app.UseMiddleware<ApiLoggingMiddleware>();

Settings which can be specified for logger in appsettings.json in "Logging" section

	"Logging": {
		"IgnoredPaths": [
		{
			"Method": "POST",
			"Path": "/api/Consents"
		},
		{
			"Method": "GET",
			"Path": "/api/Test"
		}
		],
		"LogsMaximumLengh": 0
	}
- IgnoredPaths - you can specify HTTP method and Path with which request starts
- LogsMaximumLength - if you would like to cut request and response body reflected in logs in case they are super huge
