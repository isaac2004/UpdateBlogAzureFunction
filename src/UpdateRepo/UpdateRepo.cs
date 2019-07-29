using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Alm.Authentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;

namespace UpdateRepo
{
    public class UpdateRepo
    {

        private readonly ConfigWrapper config;
        private readonly ILogger log;

        public UpdateRepo(ILoggerFactory fact, ConfigWrapper _config)
        {
            log = fact.CreateLogger("UpdateRepo");
            config = _config;
        }
        private CredentialsHandler Credentials
        {
            get
            {
                return (_url, _user, _cred) =>
                {
                    return new UsernamePasswordCredentials { Username = config.GitHubUserName, Password = config.PAT };
                };
            }
            set { }
        }
        [FunctionName("UpdateRepo")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            Event evnt = new Event();
            using (var client = new ImapClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(config.IMAPServer, Convert.ToInt32(config.IMAPPort), Convert.ToBoolean(config.IMAPUseSSL));

                client.Authenticate(config.IMAPUsername, config.IMAPPassword);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    if (message.Subject == "Update Speaking Engagement")
                    {
                        evnt = ParseMessage(message);
                        var uids = inbox.Search(SearchQuery.HeaderContains("Message-Id", message.MessageId));
                        inbox.AddFlags(uids, MessageFlags.Deleted, silent: true);
                        client.Inbox.Expunge();
                        break;
                    }
                }

                client.Disconnect(true);
            }
            if (!string.IsNullOrEmpty(evnt.EventName))
            {
                DownloadGitRepo();
                CommitRepo(evnt);
                PushRepo();
            }
        }

        private Event ParseMessage(MimeMessage message)
        {
            Dictionary<string, string> keyValuePairs = message.TextBody.Split("\r\n")
              .Where(a => a != "")
              .Select(value => value.Split('|'))
              .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());

            return JsonConvert.DeserializeObject<Event>(JsonConvert.SerializeObject(keyValuePairs));
        }

        private void DownloadGitRepo()
        {
            var secrets = new SecretStore("git");
            var auth = new BasicAuthentication(secrets);
            var creds = auth.GetCredentials(new TargetUri(config.RepoUrl));

            var options = new CloneOptions
            {
                CredentialsProvider = Credentials
            };

            if (Directory.Exists(config.RepoPath))
            {
                Directory.Delete(config.RepoPath, true);
            }

            Repository.Clone(config.RepoUrl, config.RepoPath, options);
        }


        private void CommitRepo(Event evnt)
        {
            using (var repo = new Repository(config.RepoPath))
            {
                var file = File.ReadAllText(config.RepoPath + config.DataPath);
                List<Event> data = JsonConvert.DeserializeObject<List<Event>>(file);

                data.Add(evnt);

                File.WriteAllText(config.RepoPath + config.DataPath, JsonConvert.SerializeObject(data, Formatting.Indented));

                Commands.Stage(repo, "*");

                Signature author = new Signature(config.GitHubUserName, config.GitHubAccountName, DateTime.Now);
                Signature committer = author;

                Commit commit = repo.Commit("Update Speaking Events from Email", author, committer);
            }
        }

        private void PushRepo()
        {
            using (var repo = new Repository(config.RepoPath))
            {
                LibGit2Sharp.PushOptions pushOptions = new LibGit2Sharp.PushOptions();
                pushOptions.CredentialsProvider = Credentials;
                repo.Network.Push(repo.Branches["master"], pushOptions);
            }
        }
    }
}