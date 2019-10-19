namespace MuziqGrabber.WpfClient.Model {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using MuziqGrabber.Providers.BirMuziq;

    public class SourceService : ISourceService {
        public ICollection<SourceModel> GetSources() {
            var sources = new ObservableCollection<SourceModel> {
                new SourceModel("BirMuziq", typeof (BirMuziqGrabber).AssemblyQualifiedName)
            };

            return sources;
        }
    }
}