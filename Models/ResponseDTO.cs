using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qualtrics_surveys.Models
{
    public class ResponseDTO
    {
        public string sso_id { get; set; }
        public string survey_id { get; set; }
        public bool is_updated { get; set; }
    }
}
