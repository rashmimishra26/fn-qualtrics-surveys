using Microsoft.Extensions.Options;
using System.Text.Json;
using qualtrics_surveys.Interfaces;
using qualtrics_surveys.Models;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace qualtrics_surveys.Implementation
{
    public class Auth : IAuth
    {
       
        private IOptions<AzureSettings> appSettings;
        private IConnect2Azure connect2Azure;

        public Auth(IConnect2Azure connect, IOptions<AzureSettings> config)
        {
            connect2Azure = connect;
            appSettings = config;
        }
        public async Task<(string Token, int Expire)> ReGenerateToken(Stopwatch resetTimer)
        {
            resetTimer.Restart();
            (string Token, int Expire) = await GenerateToken();
            return (Token, Expire);
        }
        public async Task<(string Token, int Expire)> GenerateToken()
        {
            Token body = default;
            var tokenApi = connect2Azure.GetSecrets(appSettings.Value.tokenApi).Result?.Value;
            tokenApi = tokenApi ?? throw new UriFormatException(nameof(tokenApi));

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{connect2Azure.GetSecrets(appSettings.Value.clientId).Result?.Value}:{ connect2Azure.GetSecrets(appSettings.Value.clientSecret).Result?.Value}");
            string userNamePasswordEncodedBase64 = System.Convert.ToBase64String(plainTextBytes);

            using (var client = new RestClient(tokenApi))
            {
                RestRequest request = new RestRequest(tokenApi, Method.Post);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", $"Basic {userNamePasswordEncodedBase64}");
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("scope", "manage:all");
                var response = await client.ExecuteAsync(request);
                body = (response.StatusCode != HttpStatusCode.BadRequest || response.StatusCode == 0) ? JsonSerializer.Deserialize<Token>(response?.Content) : throw new Exception("BadRequest");
            }

            return (body.access_token, body.expires_in);
        }
    }
}