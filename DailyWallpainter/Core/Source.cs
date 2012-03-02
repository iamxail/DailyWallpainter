using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net;
using DailyWallpainter.UI;

namespace DailyWallpainter
{
    public class Source
    {
        public Source(string name, string url, string regexp, string replacement, bool enabled, string lastBitmapUrl)
        {
            this.name = name;
            this.url = url;
            this.regexpStr = regexp;
            this.replacement = replacement;
            this.enabled = enabled;
            this.lastBitmapUrl = lastBitmapUrl;

            this.regexp = new Regex(regexpStr);
        }

        public Source(string name, string url, string regexp, string replacement)
            : this (name, url, regexp, replacement, true, "")
        {
            //do nothing
        }

        public Source(frmEditSource editor)
            : this(editor.SourceName, editor.Url, editor.RegExp, editor.Replacement, true, "")
        {
            //do nothing
        }

        public static Source[] GetSourcesFromString(string sourcesString)
        {
            try
            {
                var splitted = sourcesString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var sources = new List<Source>();

                for (int i = 0; i < splitted.Length - 1; i += 5)
                {
                    var regexpSplitted = splitted[i + 2].Split('\t');
                    if (regexpSplitted.Length == 1)
                    {
                        regexpSplitted = new string[] { regexpSplitted[0], "$1" };
                    }

                    sources.Add(new Source(splitted[i], splitted[i + 1], regexpSplitted[0], regexpSplitted[1], Convert.ToBoolean(splitted[i + 3]), splitted[i + 4]));
                }

                return sources.ToArray();
            }
            catch
            {
                return new Source[0];
            }
        }

        protected string name;
        protected string url;
        protected string regexpStr;
        protected string replacement;
        protected bool enabled;
        protected Regex regexp;
        protected string lastBitmapUrl;

        public string Name { get { return name; } }
        public string Url { get { return url; } }
        public string RegExp { get { return regexpStr; } }
        public string Replacement { get { return replacement; } }
        public bool Enabled { get { return enabled; } }
        public string LastBitmapUrl { get { return lastBitmapUrl; } }

        public override string ToString()
        {
            return name + "\r\n" + url + "\r\n" + regexpStr + "\t" + replacement + "\r\n" + enabled.ToString() + "\r\n" + lastBitmapUrl;
        }

        public SourceBitmap GetBitmap(bool force = false)
        {
            byte[] result = null;

            try
            {
                using (WebClient client = new WebClient())
                {
                    string html = client.DownloadString(url);

                    var match = regexp.Match(html);
                    if (match.Success)
                    {
                        string bitmapUrl = match.Result(replacement);

                        if (force ||
                            (lastBitmapUrl != bitmapUrl))
                        {
                            result = client.DownloadData(bitmapUrl);

                            lastBitmapUrl = bitmapUrl;
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }

            if (result != null)
            {
                return new SourceBitmap(this, result);
            }
            else
            {
                return null;
            }
        }
    }
}
