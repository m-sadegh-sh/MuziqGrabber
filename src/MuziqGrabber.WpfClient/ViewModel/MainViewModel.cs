namespace MuziqGrabber.WpfClient.ViewModel {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    using Microsoft.Practices.ServiceLocation;

    using MuziqGrabber.Core.DuplicationChecking;
    using MuziqGrabber.Core.Providers;
    using MuziqGrabber.WpfClient.Constants;
    using MuziqGrabber.WpfClient.Model;
    using MuziqGrabber.WpfClient.Properties;

    public class MainViewModel : ViewModelBase {
        private bool _isStarted;
        private bool _isPreparing;
        private bool _isCancelled;
        private ICollection<string> _cacheFilePaths;
        private IGrabber _grabber;
        private int _currentProgress;
        private ICollection<SourceModel> _sources;
        private SourceModel _selectedSource;
        private ICollection<LogModel> _logs;

        public bool IsSourceSelected {
            get { return SelectedSource != null; }
        }

        public bool IsDuplicateCheckDirectorySelected {
            get { return DuplicateCheckDirectories != null && DuplicateCheckDirectories.Count > 0; }
        }

        public bool IsStarted {
            get { return _isStarted; }
            private set {
                if (value == _isStarted)
                    return;

                _isStarted = value;
                RaisePropertyChanged(() => IsStarted);

                if (IsStarted) {
                    IsPreparing = false;
                    IsCancelled = false;
                }
            }
        }

        public bool IsPreparing {
            get { return _isPreparing; }
            private set {
                if (value == _isPreparing)
                    return;

                _isPreparing = value;
                RaisePropertyChanged(() => IsPreparing);

                if (IsPreparing) {
                    IsStarted = false;
                    IsCancelled = false;
                }
            }
        }

        public bool IsCancelled {
            get { return _isCancelled; }
            private set {
                if (value == _isCancelled)
                    return;

                _isCancelled = value;
                RaisePropertyChanged(() => IsCancelled);

                if (IsCancelled) {
                    IsPreparing = false;
                    IsStarted = false;
                }
            }
        }

        public ICollection<string> DuplicateCheckDirectories {
            get { return _cacheFilePaths; }
            set {
                _cacheFilePaths = value;
                RaisePropertyChanged(() => DuplicateCheckDirectories);
            }
        }

        public int CurrentProgress {
            get { return _currentProgress; }
            private set {
                if (value == _currentProgress)
                    return;

                _currentProgress = value;
                RaisePropertyChanged(() => CurrentProgress);
            }
        }

        public ICollection<SourceModel> Sources {
            get { return _sources; }
            set {
                _sources = value;
                RaisePropertyChanged(() => Sources);
            }
        }

        public SourceModel SelectedSource {
            get { return _selectedSource; }
            set {
                if (value == _selectedSource)
                    return;

                _selectedSource = value;
                RaisePropertyChanged(() => SelectedSource);
                RaisePropertyChanged(() => IsSourceSelected);
            }
        }

        public ICollection<LogModel> Logs {
            get { return _logs; }
            set {
                _logs = value;
                RaisePropertyChanged(() => Sources);
            }
        }

        public RelayCommand BrowseCommand { get; private set; }

        public RelayCommand StartCommand { get; private set; }

        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand ExportCommand { get; private set; }

        public MainViewModel(ISourceService sourceService) {
            Sources = sourceService.GetSources();
            StartCommand = new RelayCommand(OnStart, OnCanStart);
            CancelCommand = new RelayCommand(OnCancel, OnCanCancel);
            BrowseCommand = new RelayCommand(OnBrowse, OnCanBrowse);
            ExportCommand = new RelayCommand(OnExport, OnCanExport);
            Logs = new ObservableCollection<LogModel>();
            Messenger.Default.Register<NotificationMessage>(this, MessagingTokens.SetDuplicatePath, OnSetDuplicatePath);
            Messenger.Default.Register<NotificationMessage>(this, MessagingTokens.SetExportPath, OnSetExportPath);
            DuplicateCheckDirectories = new ObservableCollection<string>(Settings.Default.DuplicateCheckDirectories.OfType<string>());
        }

        private void OnStart() {
            IsStarted = true;

            _grabber = ServiceLocator.Current.GetInstance<IGrabber>(SelectedSource.TypeName);
            _grabber.ProgressReport += GrabberOnProgressReport;
            _grabber.ProgressChanged += GrabberOnProgressChanged;
            _grabber.GrabbingFinished += GrabberOnGrabbingFinished;

            _grabber.StartAsync(new DirectoryCheckStrategy(_cacheFilePaths));
        }

        private void GrabberOnProgressReport(object sender, GrabberProgressReportEventArgs e) {
            Logs.Add(LogModel.Info(e.DownloadUri.ToString(), true));
        }

        private void GrabberOnProgressChanged(object sender, GrabberProgressChangedEventArgs e) {
            CurrentProgress = e.CurrentProgress;
        }

        private void GrabberOnGrabbingFinished(object sender, EventArgs e) {
            IsStarted = false;
            CommandManager.InvalidateRequerySuggested();
            _grabber.ProgressReport -= GrabberOnProgressReport;
            _grabber.ProgressChanged -= GrabberOnProgressChanged;
            _grabber.GrabbingFinished -= GrabberOnGrabbingFinished;
            OnExport();
            Application.Current.Shutdown();
        }

        private bool OnCanStart() {
            return IsSourceSelected && IsDuplicateCheckDirectorySelected && !IsPreparing && !IsStarted;
        }

        private void OnCancel() {
            IsCancelled = true;
            Logs.Add(LogModel.Error("Operation cancelled by user.", false));
            CurrentProgress = 0;
        }

        private bool OnCanCancel() {
            return IsStarted;
        }

        private void OnBrowse() {
            Messenger.Default.Send(new NotificationMessage(null), MessagingTokens.GetDuplicatePath);
        }

        private bool OnCanBrowse() {
            return !IsStarted;
        }

        private void OnSetDuplicatePath(NotificationMessage message) {
            DuplicateCheckDirectories.Add(message.Notification);
        }

        private void OnExport() {
            Messenger.Default.Send(new NotificationMessage(null), MessagingTokens.GetExportPath);
        }

        private void OnSetExportPath(NotificationMessage message) {
            if (File.Exists(message.Notification))
                File.Delete(message.Notification);

            var contents = new StringBuilder();

            foreach (var log in Logs.Where(l => l.IsExportable)) {
                contents.AppendLine("<");
                contents.AppendLine(log.Description);
                contents.AppendLine(">");
            }

            File.WriteAllText(message.Notification, contents.ToString());
        }

        private bool OnCanExport() {
            return IsDuplicateCheckDirectorySelected && Logs.Any(l => l.IsExportable);
        }

        public override void Cleanup() {
            var paths = new StringCollection();
            foreach (var directory in DuplicateCheckDirectories)
                paths.Add(directory);

            Settings.Default.DuplicateCheckDirectories = paths;
            Settings.Default.Save();
        }
    }
}