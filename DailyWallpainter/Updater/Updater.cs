using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DailyWallpainter.Updater
{
    abstract class Updater : IUpdater
    {
        protected void Update(string url)
        {
            using (var client = new WebClient())
            {
                //todo
            }
        }
    }
}
