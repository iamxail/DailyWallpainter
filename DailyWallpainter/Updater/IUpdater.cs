using System;
namespace DailyWallpainter.Updater
{
    public interface IUpdater
    {
        void CheckAsync();
        void CheckAsync(object userState);

        void Update(bool silent);

        event CheckCompletedEventHandler CheckCompleted;

        bool IsChecked { get; }
        bool IsNewVersionAvailable { get; }
        string LatestVersion { get; }
    }

    public delegate void CheckCompletedEventHandler(object sender, CheckCompletedEventArgs e);

    public class CheckCompletedEventArgs : EventArgs
    {
        public CheckCompletedEventArgs(object userState, Exception ex)
        {
            UserState = userState;
            Error = ex;
        }

        public object UserState { get; protected set; }
        public Exception Error { get; protected set; }
    }
}
