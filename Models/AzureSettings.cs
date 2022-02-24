using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qualtrics_surveys.Models
{
    public class AzureSettings
    {
        public string keyVaultUrl { get; set; }
        public string surveyId { get; set; }
        public string sqlConn { get; set; }
        public string getSalesRepVw { get; set; }
        public string updateApi { get; set; }
        public string tokenApi { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
}
