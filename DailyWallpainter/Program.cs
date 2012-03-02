using System;
using System.Windows.Forms;
using DailyWallpainter.Helpers;
using System.IO;

namespace DailyWallpainter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (ArgumentExists("/forcestart") == false
                && SingleInstanceProgram.IsSingleInstaced() == false)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Context = new MainApplicationContext();

            Application.Run(Context);

            SingleInstanceProgram.Release();
        }

        public static MainApplicationContext Context { get; private set; }
        public const string Name = "Daily Wallpainter";
        public const string SafeName = "DailyWallpainter";
        public const string ExeName = SafeName + ".exe";
        public readonly static string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name);

        public static bool ArgumentExists(string argToTest)
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg.ToLower() == argToTest)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
