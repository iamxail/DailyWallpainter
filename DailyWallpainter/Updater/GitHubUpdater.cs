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
        public GitHubUpdater(string username, string repoName)
        {
            Username = username;
            RepositoryName = repoName;

            IsChecked = false;
            IsNewVersionAvailable = false;
            LatestVersion = Program.Version;
        }

        public string Username { get; protected set; }
        public string RepositoryName { get; protected set; }
        public override string LatestVersion { get; protected set; }
        public override bool IsNewVersionAvailable { get; protected set; }
        public override bool IsChecked { get; protected set; }
        public override event CheckCompletedEventHandler CheckCompleted = delegate { };

        protected string newExeUrl;

        private string GetRepoDownloadsUrl()
        {
            return "https://api.github.com/repos/" + Username + "/" + RepositoryName + "/releases";
        }

        private void CheckInternal(string downloaded)
        {
            Match latestItem = null;
            Version latestItemVersion = null;
            foreach (Match matchedItem in Regex.Matches(downloaded, "\"tag_name\"\\s*?:\\s*?\"([0-9.]+)\"[\\s\\S]*\"browser_download_url\"\\s*?:\\s*?\"([A-z0-9:.\\/]*?\\.\\1\\.exe)\""))
            {
                var matchedItemVersion = new Version(matchedItem.Groups[1].Value);

                if (latestItem == null)
                {
                    latestItem = matchedItem;
                    latestItemVersion = matchedItemVersion;
                }
                else
                {
                    if (matchedItemVersion.CompareTo(latestItemVersion) > 0)
                    {
                        latestItem = matchedItem;
                        latestItemVersion = matchedItemVersion;
                    }
                }
            }

            if (latestItem != null
                && latestItemVersion.CompareTo(new Version(LatestVersion)) > 0)
            {
                IsNewVersionAvailable = true;
                LatestVersion = latestItemVersion.GetSimpleVersionString();
                newExeUrl = latestItem.Groups[2].Value;
            }
            else
            {
                IsNewVersionAvailable = false;
                LatestVersion = Program.Version;
            }

            IsChecked = true;
        }

        public override void CheckAsync(object userState)
        {
            var client = new WebClient();
            client.Headers["User-Agent"] = Program.SafeName;
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
