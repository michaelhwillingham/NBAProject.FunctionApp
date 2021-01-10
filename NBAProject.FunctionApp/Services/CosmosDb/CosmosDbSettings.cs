namespace NBAProject.FunctionApp.Services.CosmosDb
{
    public class CosmosDbSettings
    {
        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ProdDbName { get; set; }

        public string TestDbName { get; set; }
    }
}