using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace qualtrics_surveys.Models
{
    internal class RequestDTO
    {
        public string surveyId { get; set; }
        public bool resetRecordedDate { get; set; }
        public EmbeddedData embeddedData { get; set; }

    }

    internal class EmbeddedData
    {
        [JsonPropertyName("salesRep")]
        public string sale_rep { get; set; }
    }
}
