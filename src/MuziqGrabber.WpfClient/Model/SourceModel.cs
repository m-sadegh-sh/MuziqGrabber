namespace MuziqGrabber.WpfClient.Model {
    public class SourceModel {
        public SourceModel(string title, string typeName) {
            Title = title;
            TypeName = typeName;
        }

        public string Title { get; private set; }
        public string TypeName { get; private set; }
    }
}