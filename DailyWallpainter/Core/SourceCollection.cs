using System;
using System.Collections.Generic;
using System.Text;

namespace DailyWallpainter
{

    public class SourceCollection : IEnumerable<Source>
    {
        protected List<Source> list;

        internal SourceCollection()
        {
            if (list == null)
            {
                list = new List<Source>();
            }

            Initialize();
        }

        internal SourceCollection(string from)
        {
            list = new List<Source>(Source.GetSourcesFromString(from));

            Initialize();
        }

        public void Initialize(bool force = false)
        {
            if (force)
            {
                list.Clear();
            }

            if (list.Count <= 0)
            {
                AddRange(new Source[] {
                    new Source("National Geographic - Photo of the Day",
                        @"http://photography.nationalgeographic.com/photography/photo-of-the-day/",
                        "class=\"primary_photo\"(?>\\r\\n|[\\r\\n]|.)*?<div class=\"download_link\"><a href=\"(.*?)\"|title=\"Go to the previous Photo of the Day\">(?>\\r\\n|[\\r\\n]|.)*?<img src=\"(.*?)\"",
                        "$1$2"),
                    new Source("National Geographic - Photo of the Day (High Quality Only, Not Daily)",
                        @"http://photography.nationalgeographic.com/photography/photo-of-the-day/",
                        "<div class=\"download_link\"><a href=\"(.*?)\"",
                        "$1",
                        false, ""),
                    new Source("NASA - Astronomy Picture of the Day",
                        @"http://apod.nasa.gov/apod/",
                        "<a href=\"image/(.*?)\">",
                        "http://apod.nasa.gov/apod/image/$1")
                    });
            }
        }

        public void Add(Source source)
        {
            list.Add(source);
            Save();
        }

        public void AddRange(IEnumerable<Source> sources)
        {
            list.AddRange(sources);
            Save();
        }

        public void Replace(int index, Source source)
        {
            list.RemoveAt(index);
            list.Insert(index, source);
            Save();
        }

        public void Remove(Source source)
        {
            list.Remove(source);
            Save();
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
            Save();
        }

        public void Clear()
        {
            list.Clear();
            Save();
        }

        public IEnumerable<Source> GetEnabledSources()
        {
            var enableds = new List<Source>();

            foreach (var s in list)
            {
                if (s.Enabled)
                {
                    enableds.Add(s);
                }
            }

            return enableds;
        }

        public Source this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public void Save()
        {
            Settings.Set("Sources", this.ToString());
        }

        public IEnumerator<Source> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public override string ToString()
        {
            var results = new List<string>();

            foreach (var s in list)
            {
                results.Add(s.ToString());
            }

            return string.Join("\r\n", results.ToArray());
        }
    }
}
