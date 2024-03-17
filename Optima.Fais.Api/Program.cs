using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System;

namespace Optima.Fais.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//ConfigureLoggger();
			Serilog.Log.Information("Start!!");


			try
			{
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Serilog.Log.Fatal(ex, "Host terminated unexpectedly");
			}
			finally
			{

				Serilog.Log.CloseAndFlush();

			}

			//CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				//.UseSerilog()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseIISIntegration();
					webBuilder.UseStartup<Startup>();
					webBuilder.CaptureStartupErrors(true);
					//webBuilder.UseSetting("detailedErrors", "true");
					webBuilder.UseUrls("http://localhost:7006");
					//webBuilder.UseUrls("http://10.10.20.66:8100");
					//webBuilder.UseUrls("http://192.168.100.14:8100");
				});

		public static void ConfigureLoggger()
		{
			Serilog.Log.Logger = new LoggerConfiguration()
			//.Enrich.With(new ThreadIdEnricher())
			//.WriteTo.Debug(new RenderedCompactJsonFormatter())
			.WriteTo.Console()
			//.WriteTo.Console(
			//	outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
			.WriteTo.File(new JsonFormatter(),
				"important-logs.json",
				restrictedToMinimumLevel: LogEventLevel.Warning)

			// Add a log file that will be replaced by a new log file each day
			.WriteTo.File("all-daily-.logs",
				rollingInterval: RollingInterval.Day)
			.MinimumLevel.Debug()
			//.WriteTo.File(@"log.txt", restrictedToMinimumLevel: LogEventLevel.Error)
			.CreateLogger();
		}
	}
}
