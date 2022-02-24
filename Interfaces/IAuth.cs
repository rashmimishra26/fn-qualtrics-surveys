using System.Diagnostics;
using System.Threading.Tasks;

namespace qualtrics_surveys.Interfaces
{
    public interface IAuth
    {
        public Task<(string Token, int Expire)> GenerateToken();
        public Task<(string Token, int Expire)> ReGenerateToken(Stopwatch resetTimer);
    }
}