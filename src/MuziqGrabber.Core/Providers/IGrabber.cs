namespace MuziqGrabber.Core.Providers {
    using System;

    using MuziqGrabber.Core.DuplicationChecking;

    public interface IGrabber {
        string RequestUrl { get; }
        event EventHandler<GrabberProgressReportEventArgs> ProgressReport;
        event EventHandler<GrabberProgressChangedEventArgs> ProgressChanged;
        event EventHandler<EventArgs> GrabbingFinished;
        void StartAsync(ICheckStrategy checkStrategy);
    }
}