using System;
using System.Windows.Forms;
using DailyWallpainter.Helpers;

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
            if (SingleInstanceProgram.IsSingleInstaced() == false)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Context = new MainApplicationContext();

            Application.Run(Context);
        }

        public static MainApplicationContext Context { get; private set; }
    }
}
