using System;
using System.Windows.Forms;
using DailyWallpainter.Helpers;
using System.IO;
using System.Reflection;

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
            if (ArgumentExists("/forcestart") == false)
            {
                if (SingleInstanceProgram.IsSingleInstaced() == false)
                {
                    while (SingleInstanceProgram.IsSingleInstaced() == false)
                    {
                        if (MessageBox.Show("Daily Wallpainter가 이미 실행 중입니다.\r\n\r\n계속 진행하려면 실행 중인 Daily Wallpainter를 종료한 후 재시도하십시오.", Program.Name, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error)
                            == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                }
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
        public readonly static string Version = Assembly.GetEntryAssembly().GetName().Version.GetSimpleVersionString();

        public static bool ArgumentExists(string argToTest, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                string argToTestCase = argToTest.ToLower();

                foreach (var arg in Environment.GetCommandLineArgs())
                {
                    if (arg.ToLower() == argToTestCase)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var arg in Environment.GetCommandLineArgs())
                {
                    if (arg == argToTest)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
