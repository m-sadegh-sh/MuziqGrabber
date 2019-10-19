namespace MuziqGrabber.WpfClient {
    using System.Windows;
    using System.Windows.Forms;

    using GalaSoft.MvvmLight.Messaging;

    using MuziqGrabber.WpfClient.Constants;
    using MuziqGrabber.WpfClient.ViewModel;

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<NotificationMessage>(this, MessagingTokens.GetDuplicatePath, OnGetDuplicatePath);
            Messenger.Default.Register<NotificationMessage>(this, MessagingTokens.GetExportPath, OnGetExportPath);
        }

        private static void OnGetDuplicatePath(NotificationMessage message) {
            var dialog = new FolderBrowserDialog {
                ShowNewFolderButton = true,
                SelectedPath = message.Notification
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Messenger.Default.Send(new NotificationMessage(dialog.SelectedPath), MessagingTokens.SetDuplicatePath);
        }

        private static void OnGetExportPath(NotificationMessage message) {
            var dialog = new SaveFileDialog {
                AutoUpgradeEnabled = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Messenger.Default.Send(new NotificationMessage(dialog.FileName), MessagingTokens.SetExportPath);
        }
    }
}