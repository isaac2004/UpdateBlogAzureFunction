using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateRepo
{
    /// <summary>
    /// Wrapper for Environment Variables
    /// </summary>
    /// <remarks>
    /// This class represents a Strongly-Typed instance of Environment Variables, set in Azure Portal
    /// </remarks>
    public class ConfigWrapper
    {

        public string AzureWebJobsStorage
        {
            get { return Environment.GetEnvironmentVariable("AzureWebJobsStorage"); }
        }
        public string FUNCTIONS_WORKER_RUNTIME
        {
            get { return Environment.GetEnvironmentVariable("FUNCTIONS_WORKER_RUNTIME"); }
        }

        public string IMAPServer
        {
            get { return Environment.GetEnvironmentVariable("IMAPServer"); }
        }

        public string IMAPPort
        {
            get { return Environment.GetEnvironmentVariable("IMAPPort"); }
        }

        public string IMAPUseSSL
        {
            get { return Environment.GetEnvironmentVariable("IMAPUseSSL"); }
        }

        public string IMAPUsername
        {
            get { return Environment.GetEnvironmentVariable("IMAPUsername"); }
        }

        public string IMAPPassword
        {
            get { return Environment.GetEnvironmentVariable("IMAPPassword"); }
        }

        public string RepoUrl
        {
            get { return Environment.GetEnvironmentVariable("RepoUrl"); }
        }

        public string RepoPath
        {
            get { return Environment.GetEnvironmentVariable("RepoPath"); }
        }

        public string DataPath
        {
            get { return Environment.GetEnvironmentVariable("DataPath"); }
        }

        public string PAT
        {
            get { return Environment.GetEnvironmentVariable("PAT"); }
        }
        public string GitHubUserName
        {
            get { return Environment.GetEnvironmentVariable("GitHubUserName"); }
        }
        public string GitHubAccountName
        {
            get { return Environment.GetEnvironmentVariable("GitHubAccountName"); }
        }
    }
}
