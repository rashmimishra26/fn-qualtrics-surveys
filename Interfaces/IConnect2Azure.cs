using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qualtrics_surveys.Interfaces
{
    public interface IConnect2Azure
    {
        public Task<KeyVaultSecret> GetSecrets(string secretName);
        public Task<IDbConnection> ConnectToDatabase();
    }
}
