using qualtrics_surveys.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace qualtrics_surveys.Interfaces
{
    public interface IUtil
    {
        public Task<string> UpdateSurveys();
    }
}
