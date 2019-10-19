namespace MuziqGrabber.Core.Providers {
    using System;

    public sealed class GrabberProgressChangedEventArgs : EventArgs {
        public int CurrentProgress { get; private set; }

        public GrabberProgressChangedEventArgs(int currentProgress) {
            CurrentProgress = currentProgress;
        }
    }
}