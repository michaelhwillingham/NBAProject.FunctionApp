using System.Security.Authentication;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace NBAProject.FunctionApp.Services.CosmosDb
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosDbSettings _cosmosDbSettings;
        private MongoClient MongoClient { get; }

        public CosmosDbService(IOptions<CosmosDbSettings> settings)
        {
            _cosmosDbSettings = settings.Value;

            var mongoIdentity = new MongoInternalIdentity("admin", _cosmosDbSettings.Username);
            var passwordEvidence = new PasswordEvidence(_cosmosDbSettings.Password);

            var mongoClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(_cosmosDbSettings.Host, 10255),
                UseSsl = true,
                SslSettings = new SslSettings
                {
                    EnabledSslProtocols = SslProtocols.Tls12
                },
                Credential = new MongoCredential("SCRAM-SHA-1", mongoIdentity, passwordEvidence),
                RetryWrites = false
            };

            MongoClient = new MongoClient(mongoClientSettings);
        }

        public string GetProdDbName()
        {
            return _cosmosDbSettings.ProdDbName;
        }

        public string GetTestDbName()
        {
            return _cosmosDbSettings.TestDbName;
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return MongoClient.GetDatabase(databaseName);
        }
    }

    public interface ICosmosDbService
    {
        public string GetProdDbName();

        public string GetTestDbName();

        public IMongoDatabase GetDatabase(string databaseName);
    }
}