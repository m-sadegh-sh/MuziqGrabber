namespace MuziqGrabber.Providers.BirMuziq {
    using System.Collections.Generic;
    using System.Linq;

    using MuziqGrabber.Core.Models;
    using MuziqGrabber.Core.Providers;

    public sealed class BirMuziqGrabber : GrabberBase {
        public override string RequestUrl {
            get { return "http://www.birmusic74.in/article-topic0-page{0}.html"; }
        }

        protected override string FixContentErrors(string content) {
            content = content.Replace("]--!>", "]-->");

            return content;
        }

        protected override bool IsValidPostUrl(string url) {
            int postId;
            return !string.IsNullOrEmpty(url) && int.TryParse(url.Split('-')[0], out postId);
        }

        protected override bool IsValidPostDownloadUrl(string url) {
            return url.Contains("dl.") && (url.Contains("128") || url.Contains(".zip"));
        }

        protected override IEnumerable<ParsedPostContentModel> GetCandidatePostContents(IEnumerable<ParsedPostContentModel> postContents) {
            var zip128 = postContents.Where(pc => pc.DownloadUri.AbsolutePath.EndsWith(".zip") && pc.DownloadUri.AbsolutePath.Contains("128"));
            if (zip128.Any())
                return zip128;

            var mp3128 = postContents.Where(pc => pc.DownloadUri.AbsolutePath.Contains("128"));
            if (mp3128.Any())
                return mp3128;

            return Enumerable.Empty<ParsedPostContentModel>();
        }
    }
}