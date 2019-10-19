namespace MuziqGrabber.WpfClient.Model {
    using System.Collections.Generic;

    public interface ISourceService {
        ICollection<SourceModel> GetSources();
    }
}