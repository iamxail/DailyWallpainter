using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DailyWallpainter.Helpers
{
    public static class InternetConnection
    {
        //http://stackoverflow.com/questions/20309158/c-sharp-checking-internet-connection
        public static bool IsAvailable()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
