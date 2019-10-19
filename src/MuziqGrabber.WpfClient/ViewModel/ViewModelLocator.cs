namespace MuziqGrabber.WpfClient.ViewModel {
    using System;

    using GalaSoft.MvvmLight.Ioc;

    using Microsoft.Practices.ServiceLocation;

    using MuziqGrabber.Core.Providers;
    using MuziqGrabber.WpfClient.Model;

    public class ViewModelLocator {
        static ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ISourceService, SourceService>();
            SimpleIoc.Default.Register<MainViewModel>();

            var sourceService = ServiceLocator.Current.GetInstance<ISourceService>();
            var sources = sourceService.GetSources();

            foreach (var source in sources)
                SimpleIoc.Default.Register(() => (IGrabber)Activator.CreateInstance(Type.GetType(source.TypeName)), source.TypeName);
        }

        public MainViewModel Main {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public static void Cleanup() {
            SimpleIoc.Default.GetInstance<MainViewModel>().Cleanup();
        }
    }
}