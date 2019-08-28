using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace UpdateRepo
{
    public class UpdateRepo
    {
        private readonly ConfigWrapper config;
        private readonly ILogger log;
        private string tempFolder;

        public UpdateRepo(ILoggerFactory fact, ConfigWrapper _config)
        {
            log = fact.CreateLogger("UpdateRepo");
            config = _config;
            tempFolder = $"{config.RepoPath}/temp";
        }

        [FunctionName("UpdateRepo")]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            List<Event> newEvents = ParseEmail();

            if (newEvents != null && newEvents.Count > 0)
            {
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                Directory.SetCurrentDirectory(tempFolder);

                log.LogInformation("Clone Done");
                try
                {
                    RunCmd("clone.cmd");

                    string json = File.ReadAllText($"{tempFolder}/{config.DataPath}");
                    var events = JsonConvert.DeserializeObject<List<Event>>(json);
                    events.AddRange(newEvents);
                    File.WriteAllText($"{tempFolder}/{config.DataPath}", JsonConvert.SerializeObject(events, Formatting.Indented));

                    RunCmd("commit.cmd");
                }
                catch
                {

                }
                finally
                {
                    EmptyFolder(tempFolder);
                }
            }
        }

        private List<Event> ParseEmail()
        {
            List<Event> events = new List<Event>();
            using (var client = new ImapClient())
            {
                // For demo-purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(config.IMAPServer, Convert.ToInt32(config.IMAPPort), Convert.ToBoolean(config.IMAPUseSSL));
                client.Authenticate(config.IMAPUsername, config.IMAPPassword);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);
                var query = SearchQuery.SubjectContains("Update Speaking Engagement");
                foreach (var uid in inbox.Search(query))
                {
                    var message = inbox.GetMessage(uid);
                    events.Add(ParseMessage(message.GetTextBody(MimeKit.Text.TextFormat.Plain)));
                    log.LogInformation("Subject: {0}", message.Subject);
                    inbox.AddFlags(uid, MessageFlags.Deleted, true);
                }
                client.Inbox.Expunge();
                client.Disconnect(true);
            }
            return events;
        }

        private void RunCmd(string cmdName)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = $"{config.RepoPath}/{cmdName}",
                Arguments = "",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = tempFolder
            };
            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Thread.Sleep(10000);
        }

        private bool EmptyFolder(string pathName)
        {
            bool errors = false;
            DirectoryInfo dir = new DirectoryInfo(pathName);

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                try
                {
                    fi.IsReadOnly = false;
                    fi.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (fi.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        fi.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            foreach (DirectoryInfo di in dir.EnumerateDirectories())
            {
                try
                {
                    EmptyFolder(di.FullName);
                    di.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (di.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        di.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

            try
            {
                dir.Delete();

                //Wait for the item to disapear (avoid 'dir not empty' error).
                while (dir.Exists)
                {
                    System.Threading.Thread.Sleep(10);
                    dir.Refresh();
                }
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                errors = true;
            }

            return !errors;
        }

        private Event ParseMessage(string message)
        {
            Dictionary<string, string> keyValuePairs = message.Split("\r\n")
              .Where(a => a != "")
              .Select(value => value.Split('|'))
              .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());

            return JsonConvert.DeserializeObject<Event>(JsonConvert.SerializeObject(keyValuePairs));
        }
    }
}