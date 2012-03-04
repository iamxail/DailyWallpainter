using System;
using System.Collections.Generic;
using System.Text;


namespace DailyWallpainter.Helpers
{
    public static class VersionHelper
    {
        public static string GetSimpleVersionString(this Version ver)
        {
            string verStr = ver.Major.ToString() + "." + ver.Minor.ToString();

            if (ver.Build > 0)
            {
                verStr += "." + ver.Revision.ToString() + "." + ver.Build.ToString();
            }
            else if (ver.Revision > 0)
            {
                verStr += "." + ver.Revision.ToString();
            }

            return verStr;
        }
    }
}
