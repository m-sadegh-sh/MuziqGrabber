namespace MuziqGrabber.WpfClient.Model {
    using System.Windows.Media;

    public class LogModel {
        private LogModel(string description, Color statusColor, bool isExportable) {
            Description = description;
            StatusColor = new SolidColorBrush(statusColor);
            IsExportable = isExportable;
        }

        public static LogModel Success(string description, bool isExportable) {
            return new LogModel(description, Color.FromRgb(0, 194, 0), isExportable);
        }

        public static LogModel Info(string description, bool isExportable) {
            return new LogModel(description, Color.FromRgb(0, 131, 194), isExportable);
        }

        public static LogModel Warning(string description, bool isExportable) {
            return new LogModel(description, Color.FromRgb(194, 193, 0), isExportable);
        }

        public static LogModel Error(string description, bool isExportable) {
            return new LogModel(description, Color.FromRgb(194, 0, 0), isExportable);
        }

        public string Description { get; private set; }
        public Brush StatusColor { get; private set; }
        public bool IsExportable { get; set; }
    }
}