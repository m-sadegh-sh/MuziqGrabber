namespace MuziqGrabber.WpfClient.Model {
    public class DownloadedItemModel {
        public DownloadedItemModel(SourceModel source, string title, string downloadUrl, string refererUrl, string savedLocation) {
            Source = source;
            Title = title;
            DownloadUrl = downloadUrl;
            RefererUrl = refererUrl;
            SavedLocation = SavedLocation;
        }

        public SourceModel Source { get; private set; }
        public string Title { get; private set; }
        public string DownloadUrl { get; private set; }
        public string RefererUrl { get; private set; }
        public string SavedLocation { get; private set; }
    }
}