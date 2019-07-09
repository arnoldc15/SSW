using System;
using Application;
using Application.Queries;
using Infrastructure;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]

namespace FunctionApp1
{
	// Composition Root
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			ConfigureAppSettings(builder);

			builder.Services.AddHttpClient();
			builder.Services.AddMediatR(typeof(GetReceiptByMembershipId));
			
			ConfigureServices(builder);
		}

		private static void ConfigureServices(IFunctionsHostBuilder builder)
		{
			builder.Services.AddTransient<MyReceiptBuilder>();
			builder.Services.AddTransient<IRebateApi, RealRebateApi>();
			builder.Services.AddTransient<ISubsidyApi, RealSubsidyApi>();
			builder.Services.AddTransient<IDiscountApi, RealDiscountApi>();
		}

		protected void ConfigureAppSettings(IFunctionsHostBuilder builder)
		{
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(Environment.CurrentDirectory)
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
#if DEBUG
				.AddJsonFile("local.debug.settings.json", optional: true, reloadOnChange: true)
#endif
				.AddEnvironmentVariables()
				.Build();

			builder.Services.AddSingleton(configuration);
		}
	}
}
