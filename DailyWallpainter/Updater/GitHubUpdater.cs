using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using DailyWallpainter.Helpers;

namespace DailyWallpainter.Updater
{
    public class GitHubUpdater : Updater
    {
        public GitHubUpdater(string username, string repoName, string filename)
        {
            Username = username;
            RepositoryName = repoName;
            Filename = filename;

            IsChecked = false;
            IsNewVersionAvailable = false;
            LatestVersion = Program.Version;
        }

        public string Username { get; protected set; }
        public string RepositoryName { get; protected set; }
        public string Filename { get; protected set; }
        public override string LatestVersion { get; protected set; }
        public override bool IsNewVersionAvailable { get; protected set; }
        public override bool IsChecked { get; protected set; }
        public override event CheckCompletedEventHandler CheckCompleted = delegate { };

        protected string newExeUrl;

        private string GetRepoDownloadsUrl()
        {
            return "https://api.github.com/repos/" + Username + "/" + RepositoryName + "/downloads";
        }

        private void CheckInternal(string downloaded)
        {
            try
            {
                var regexSplitter = new Regex("}[ .\r\n]*?,[ .\r\n]*?{");
                var downloadsSplitted = regexSplitter.Split(downloaded);

                var regexFilename = new Regex("\"name\" *?: *?\"" + Filename + "\"[ .\r\n]*[,}]", RegexOptions.IgnoreCase);
                foreach (var download in downloadsSplitted)
                {
                    if (regexFilename.IsMatch(download))
                    {
                        var regexDesc = new Regex("\"description\" *?: *?\"(?:version|ver|v) ?([0-9]*\\.[0-9]*)", RegexOptions.IgnoreCase);
                        var match = regexDesc.Match(download);

                        if (match.Success)
                        {
                            LatestVersion = match.Groups[1].Value;
                            IsNewVersionAvailable = (LatestVersion != Program.Version);

                            var regexUrl = new Regex("\"html_url\" *?: *?\"(.*?)\"", RegexOptions.IgnoreCase);
                            var matchUrl = regexUrl.Match(download);

                            if (matchUrl.Success)
                            {
                                newExeUrl = matchUrl.Groups[1].Value;
                            }

                            break;
                        }
                    }
                }
            }
            catch
            {
                IsNewVersionAvailable = false;
                LatestVersion = Program.Version;

                throw;
            }
            finally
            {
                IsChecked = true;
            }
        }

        public override void CheckAsync(object userState)
        {
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(GetRepoDownloadsUrl()), new object[] { client, userState });
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var client = (e.UserState as object[])[0] as WebClient;
            Exception ex = null;

            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }

                CheckInternal(e.Result);
            }
            catch (Exception thrown)
            {
                ex = thrown;
            }
            finally
            {
                try
                {
                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
                catch
                {
                }

                OnCheckCompleted((e.UserState as object[])[1], ex);
            }
        }

        protected virtual void OnCheckCompleted(object userState, Exception ex)
        {
            CheckCompleted(this, new CheckCompletedEventArgs(userState, ex));
        }

        public override void UpdateAsync(bool silent, object userState)
        {
            UpdateAsync(newExeUrl, Program.SafeName + "_" + LatestVersion + ".exe", silent, userState);
        }
    }
}
