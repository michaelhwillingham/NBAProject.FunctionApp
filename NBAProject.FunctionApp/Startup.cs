using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NBAProject.FunctionApp.Services.CosmosDb;
using NBAProject.FunctionApp.Services.MySportsFeedsApi;

[assembly: FunctionsStartup(typeof(NBAProject.FunctionApp.Startup))]

namespace NBAProject.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            builder.Services.AddOptions<MySportsFeedsApiSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("MySportsFeedsApiSettings").Bind(settings);
                });

            builder.Services.AddSingleton<IMySportsFeedsApiService, MySportsFeedsApiService>();

            builder.Services.AddOptions<CosmosDbSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("CosmosDbSettings").Bind(settings);
                });

            builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();

            builder.Services.AddMediatR(typeof(Startup));
        }
    }
}