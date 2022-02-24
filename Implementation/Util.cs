using Microsoft.Extensions.Options;
using qualtrics_surveys.Interfaces;
using qualtrics_surveys.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;

namespace qualtrics_surveys.Implementation
{
    public class Util : IUtil
    {
        private IOptions<AzureSettings> appSettings;
        private IConnect2Azure connect2Azure;
        private IAuth authToken;
        Stopwatch timer = new Stopwatch();
        public Util(IConnect2Azure connect, IOptions<AzureSettings> config, IAuth auth)
        {
            connect2Azure = connect;
            appSettings = config;
            authToken = auth;
        }

        public async Task<string> UpdateSurveys()
        {
            List<ResponseDTO> responseList = new List<ResponseDTO>();

            try
            {
                DataTable dtSurvey = FetchSurveys().Result.Tables[0];
                string updateApi = connect2Azure.GetSecrets(appSettings.Value.updateApi).Result?.Value;
                updateApi = updateApi ?? throw new UriFormatException(nameof(updateApi));

                var token = authToken.GenerateToken().Result;
                timer.Start();

                /*foreach (DataRow dr in dtSurvey.Rows)
                {
                    token.Token = (timer.Elapsed.TotalSeconds) != token.Expire ? token.Token : authToken.ReGenerateToken(timer)?.Result.Token;
                    responseList.Add(await UpdateSurvey(dr, updateApi, token.Token));
                }*/

                Parallel.ForEach(dtSurvey.AsEnumerable(), async dr =>
                {
                    token.Token = (timer.Elapsed.TotalSeconds) != token.Expire ? token.Token : authToken.ReGenerateToken(timer)?.Result.Token;
                    responseList.Add(await UpdateSurvey(dr, updateApi, token.Token));
                });
            }
            catch (UriFormatException uriex)
            {
                throw uriex;
            }

            catch (SqlException sqlex)
            {
                throw sqlex;
            }
            catch (Exception ex)
            {
                throw;
            }
            
            return JsonSerializer.Serialize(responseList.Where(qry => qry.is_updated == false));
        }

        #region private methods

        private Task<DataSet> FetchSurveys()
        {
            DataSet dsSurvey = new DataSet();

            using (var conn = (SqlConnection)connect2Azure.ConnectToDatabase().Result)
            {
                using (var command = new SqlCommand($"select * from {appSettings.Value.getSalesRepVw}", conn))
                {
                    command.CommandType = CommandType.Text;
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(dsSurvey);
                    }
                }
            }

            return Task.FromResult(dsSurvey);
        }

        private async Task<ResponseDTO> UpdateSurvey(DataRow dr, string updateApi, string authToken)
        {
            ResponseDTO dtoResponse = default;

            var requestDto = new RequestDTO {
                surveyId = appSettings.Value.surveyId,
                resetRecordedDate = true,
                embeddedData = new EmbeddedData { sale_rep = dr["sales_rep"].ToString() }
            };

            updateApi += dr["response_id"].ToString();
            
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(updateApi),
                Headers = {
                            {"Access-Control-Allow-Headers","*" },
                            {"Authorization", $"Bearer {authToken}"},
                            //{"Access-Control-Allow-Origin","http://localhost:7071" },
                            { "Access-Control-Allow-Methods","PUT,POST"}
                           },
                Content = new StringContent(JsonSerializer.Serialize<RequestDTO>(requestDto))
                {
                    Headers = {
                                            ContentType = new MediaTypeHeaderValue("application/json")
                                        }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                var body = await response.Content.ReadAsStringAsync();
                
                dtoResponse = new ResponseDTO
                {
                    sso_id = dr["sso_user_id"].ToString(),
                    survey_id = requestDto.surveyId,
                    is_updated = (response.StatusCode is 0 or (HttpStatusCode)400) ? Convert.ToBoolean(0) : Convert.ToBoolean(1)
                };
            }
            

            return dtoResponse;
        }



        #endregion
    }
}
