namespace MuziqGrabber.Core.Models {
    using System;

    public sealed class ParsedPostContentModel : ModelBase {
        public string Title { get; set; }
        public Uri DownloadUri { get; set; }
    }
}