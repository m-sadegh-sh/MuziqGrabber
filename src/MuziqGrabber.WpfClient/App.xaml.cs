namespace MuziqGrabber.WpfClient {
    using System.Collections.Specialized;
    using System.Windows;

    using GalaSoft.MvvmLight.Threading;

    using MuziqGrabber.WpfClient.Properties;

    public partial class App : Application {
        static App() {
            DispatcherHelper.Initialize();

            if (Settings.Default.DuplicateCheckDirectories == null) {
                Settings.Default.DuplicateCheckDirectories = new StringCollection();
                Settings.Default.Save();
            }
        }
    }
}