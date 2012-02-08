using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DailyWallpainter.UpdateChecker
{
    public class GitHubUpdateChecker
    {
        public GitHubUpdateChecker(string username, string repoName, string filename)
        {
            Username = username;
            RepositoryName = repoName;
            Filename = filename;
        }

        public string Username { get; protected set; }
        public string RepositoryName { get; protected set; }
        public string Filename { get; protected set; }
        public string LatestVersion { get; protected set; }

        private string GetMajorDotMinorVersion()
        {
            var ver = Assembly.GetEntryAssembly().GetName().Version;

            return ver.Major.ToString() + "." + ver.Minor.ToString();
        }

        public bool IsNewVersionAvailable()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var downloads = client.DownloadString("https://api.github.com/repos/" + Username + "/" + RepositoryName + "/downloads");

                    var regexSplitter = new Regex("}[ .\r\n]*?,[ .\r\n]*?{");
                    var downloadsSplitted = regexSplitter.Split(downloads);

                    var regexFilename = new Regex("\"name\" *?: *?\"" + Filename + "\"[ .\r\n]*[,}]", RegexOptions.IgnoreCase);
                    var nowVer = GetMajorDotMinorVersion();
                    var isNewVersion = false;
                    foreach (var download in downloadsSplitted)
                    {
                        if (regexFilename.IsMatch(download))
                        {
                            var regexDesc = new Regex("\"description\" *?: *?\"(?:version|ver|v) ?([0-9]*\\.[0-9]*)", RegexOptions.IgnoreCase);
                            var match = regexDesc.Match(download);

                            if (match.Success)
                            {
                                LatestVersion = match.Groups[1].Value;
                                isNewVersion = (LatestVersion != nowVer);

                                break;
                            }
                        }
                    }

                    return isNewVersion;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
