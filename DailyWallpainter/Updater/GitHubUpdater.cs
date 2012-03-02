﻿using System;
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
        public override event CheckCompletedEventHandler CheckCompleted;

        protected string newExeUrl;

        public override void CheckAsync()
        {
            CheckAsync(null);
        }

        public override void CheckAsync(object userState)
        {
            var client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri("https://api.github.com/repos/" + Username + "/" + RepositoryName + "/downloads"), new object[] { client, userState });
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

                var downloads = e.Result;

                var regexSplitter = new Regex("}[ .\r\n]*?,[ .\r\n]*?{");
                var downloadsSplitted = regexSplitter.Split(downloads);

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
            catch (Exception thrown)
            {
                ex = thrown;

                IsNewVersionAvailable = false;
                LatestVersion = Program.Version;
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

                IsChecked = true;
                OnCheckCompleted((e.UserState as object[])[1], ex);
            }
        }

        private void OnCheckCompleted(object userState, Exception ex)
        {
            if (CheckCompleted != null)
            {
                CheckCompleted(this, new CheckCompletedEventArgs(userState, ex));
            }
        }

        public override void Update(bool silent)
        {
            Update(newExeUrl, Program.SafeName + "_" + LatestVersion + ".exe", silent);
        }
    }
}
