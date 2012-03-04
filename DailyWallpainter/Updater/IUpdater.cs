using System;
namespace DailyWallpainter.Updater
{
    public interface IUpdater
    {
        void CheckAsync();
        void CheckAsync(object userState);

        void UpdateAsync(bool silent);
        void UpdateAsync(bool silent, object userState);

        event CheckCompletedEventHandler CheckCompleted;
        event UpdateCompletedEventHandler UpdateCompleted;

        bool IsChecked { get; }
        bool IsNewVersionAvailable { get; }
        string LatestVersion { get; }
    }

    public delegate void UpdateCompletedEventHandler(object sender, UpdateCompletedEventArgs e);
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

    public class UpdateCompletedEventArgs : EventArgs
    {
        public UpdateCompletedEventArgs(object userState, Exception ex)
        {
            UserState = userState;
            Error = ex;
        }

        public object UserState { get; protected set; }
        public Exception Error { get; protected set; }
    }
}
