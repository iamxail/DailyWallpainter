using System;
namespace DailyWallpainter.UpdateChecker
{
    public interface IUpdateChecker
    {
        void CheckAsync();
        void CheckAsync(object userState);
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
