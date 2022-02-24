using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using qualtrics_surveys.Interfaces;

namespace qualtrics_surveys
{
    public class fn_update_surveys
    {
        private IUtil util;

        public fn_update_surveys(IUtil utilobj)
        {
            util = utilobj;
        }
        
        [FunctionName("fn_update_surveys")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("function request processing started.");
            
            return new OkObjectResult(await util.UpdateSurveys());
            
            
        }
    }
}
