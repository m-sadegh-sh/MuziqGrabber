namespace MuziqGrabber.Build {
    using System.Reflection;

    public class CurrentBuild {
        public const string Company = "MuziqGrabber™";

        public const string Copyright = "Copyright © MuziqGrabber™ Inc 2014";

        public const string Trademark = "MuziqGrabber™";

        public const string Version = "0.7.*";

        public const string FileVersion = "0.7.0.0";

        public static string CurrentVersion {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
    }
}