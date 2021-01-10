using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NBAProject.FunctionApp.Services.CosmosDb;
using NBAProject.FunctionApp.Services.MySportsFeedsApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using RestSharp;

namespace NBAProject.FunctionApp.Functions.Teams.Update
{
    public class Request : IRequest<Unit>
    {
        public string Season { get; set; }
    }

    public class RequestHandler : IRequestHandler<Request, Unit>
    {
        private readonly IMySportsFeedsApiService _mySportsFeedsApiService;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<RequestHandler> _logger;

        public RequestHandler(
            IMySportsFeedsApiService mySportsFeedsApiService,
            ICosmosDbService cosmosDbService,
            ILogger<RequestHandler> logger)
        {
            _mySportsFeedsApiService = mySportsFeedsApiService;
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiving data from MySportsFeed API...");
            var apiResponse = await GetTeamsDataFromApi(request.Season);
            _logger.LogInformation("MySportsFeed API call successful.");

            _logger.LogInformation("Parsing API response schema...");
            var responseSchema = JSchema.Parse(apiResponse);
            _logger.LogInformation("API response schema:");
            _logger.LogInformation($"{JsonConvert.SerializeObject(responseSchema, Formatting.Indented)}");

            var databaseNames = new[]
            {
                _cosmosDbService.GetProdDbName(),
                _cosmosDbService.GetTestDbName()
            };

            foreach (var databaseName in databaseNames)
            {
                _logger.LogInformation($"Updating players on {databaseName}...");
                await UpdateTeamsInMongo(databaseName, apiResponse);
                _logger.LogInformation($"Successfully updated players on {databaseName}.");
            }

            return new Unit();
        }

        private async Task<string> GetTeamsDataFromApi(string season)
        {
            return await _mySportsFeedsApiService.GetAsync(new RestRequest
            {
                Resource = $"{season}/standings.json",
                Method = Method.GET,
                RequestFormat = DataFormat.Json,
                AllowedDecompressionMethods = {DecompressionMethods.GZip}
            });
        }

        private async Task UpdateTeamsInMongo(string databaseName, string json)
        {
            var database = _cosmosDbService.GetDatabase(databaseName);

            var teamsCollection = database.GetCollection<BsonDocument>("teams");

            await teamsCollection.DeleteManyAsync(new BsonDocument(), CancellationToken.None);

            var documentToInsert = BsonDocument.Parse(json);

            await teamsCollection.InsertOneAsync(documentToInsert);
        }
    }
}