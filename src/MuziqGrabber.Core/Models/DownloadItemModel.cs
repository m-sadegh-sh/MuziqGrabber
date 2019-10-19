namespace MuziqGrabber.Core.Models {
    public sealed class DownloadItemModel : ModelBase {
        public string Title { get; set; }
        public string DownloadUrl { get; set; }
        public string SavedLocation { get; set; }
        public string RefererUrl { get; set; }
    }
}