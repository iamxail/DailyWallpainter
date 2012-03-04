using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DailyWallpainter.Helpers
{
    public static class SafeFilename
    {
        public static string Convert(string directory, string filename, bool ignoreFileExists = false)
        {
            string safeFilename = filename;

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                safeFilename = safeFilename.Replace(c, '_');
            }

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            if (ignoreFileExists)
            {
                return Path.Combine(directory, safeFilename);
            }
            else
            {
                string ext = Path.GetExtension(safeFilename);
                string safePathWithoutExt = Path.Combine(directory, safeFilename.Substring(0, safeFilename.Length - ext.Length));

                string suffix = "";
                int i = 1;
                while (File.Exists(safePathWithoutExt + suffix + ext))
                {
                    i++;
                    suffix = " (" + i.ToString() + ")";
                }

                return safePathWithoutExt + suffix + ext;
            }
        }
    }
}
