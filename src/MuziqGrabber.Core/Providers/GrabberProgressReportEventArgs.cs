namespace MuziqGrabber.Core.Providers {
    using System;

    public sealed class GrabberProgressReportEventArgs : EventArgs {
        public string Title { get; private set; }
        public Uri DownloadUri { get; private set; }

        public GrabberProgressReportEventArgs(string title, Uri downloadUri) {
            Title = title;
            DownloadUri = downloadUri;
        }
    }
}