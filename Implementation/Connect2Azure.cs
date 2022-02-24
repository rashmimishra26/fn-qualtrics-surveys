using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Options;
using qualtrics_surveys.Interfaces;
using qualtrics_surveys.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace qualtrics_surveys.Implementation
{
    internal class Connect2Azure : IConnect2Azure
    {
        private IOptions<AzureSettings> options = default;
        ChainedTokenCredential credential = default;
        private string kvUrl = default;
        public Connect2Azure(IOptions<AzureSettings> options)
        {
            this.options = options ?? throw new ArgumentNullException();
            kvUrl = $"https://{options?.Value.keyVaultUrl}.vault.azure.net/";

            credential = new ChainedTokenCredential(
                new ManagedIdentityCredential()
                , new DefaultAzureCredential());
        }
        public Task<IDbConnection> ConnectToDatabase() => Task.FromResult(GetDbConnection);

        public async Task<KeyVaultSecret> GetSecrets(string secretName)
        {
            try
            {
                var kvClient = new SecretClient(new Uri(kvUrl), credential);
                return await kvClient.GetSecretAsync(secretName);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private IDbConnection GetDbConnection => new SqlConnection(GetSecrets(options?.Value.sqlConn).Result.Value);
    }
}
