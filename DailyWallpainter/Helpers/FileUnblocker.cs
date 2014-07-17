//http://stackoverflow.com/questions/6375599/looks-this-pinvoke-correct-and-reliable

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace DailyWallpainter.Helpers
{
    public class FileUnblocker
    {

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        public static bool Unblock(string fileName)
        {
            return DeleteFile(fileName + ":Zone.Identifier");
        }
    }
}
