using System;

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
    }
}
