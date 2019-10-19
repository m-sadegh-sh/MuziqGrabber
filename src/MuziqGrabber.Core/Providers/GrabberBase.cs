namespace MuziqGrabber.Core.Providers {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;

    using HtmlAgilityPack;

    using MuziqGrabber.Core.DuplicationChecking;
    using MuziqGrabber.Core.Models;

    public abstract class GrabberBase : IGrabber {
        private readonly object _lock = new object();
        private const int MAX_DUPLICATE_ITEMS = 5;
        private int _foundedDuplicateItems = 0;

        public abstract string RequestUrl { get; }

        public event EventHandler<GrabberProgressReportEventArgs> ProgressReport;
        public event EventHandler<GrabberProgressChangedEventArgs> ProgressChanged;
        public event EventHandler<EventArgs> GrabbingFinished;

        public async void StartAsync(ICheckStrategy checkStrategy) {
            var client = new WebClient();

            for (var page = 1; page <= 100; page++) {
                var requestUri = new Uri(string.Format(RequestUrl, page));
                var postsRawContent = await client.DownloadStringTaskAsync(requestUri);
                postsRawContent = FixContentErrors(postsRawContent);
                var document = new HtmlDocument();
                document.LoadHtml(postsRawContent);

                var postLinks = GetPostLinks(requestUri, document);

                foreach (var postLink in postLinks) {
                    try {
                        var postRawContent = await client.DownloadStringTaskAsync(postLink.PostUri);
                        postRawContent = FixContentErrors(postRawContent);
                        document.LoadHtml(postRawContent);

                        var postContents = GetPostContents(checkStrategy, document);
                        foreach (var postContent in postContents)
                            OnProgressReport(postContent.Title, postContent.DownloadUri);

                        if (_foundedDuplicateItems > MAX_DUPLICATE_ITEMS)
                            break;
                    } catch {}
                }

                if (_foundedDuplicateItems > MAX_DUPLICATE_ITEMS)
                    break;
            }

            OnGrabbingFinished();
        }

        protected abstract string FixContentErrors(string content);
        protected abstract bool IsValidPostUrl(string url);
        protected abstract bool IsValidPostDownloadUrl(string url);
        protected abstract IEnumerable<ParsedPostContentModel> GetCandidatePostContents(IEnumerable<ParsedPostContentModel> postContents);

        private IEnumerable<ParsedPostLinkModel> GetPostLinks(Uri baseUri, HtmlDocument document) {
            var postUrls = document.DocumentNode.SelectNodes("//a[@href]").Select(n => n.Attributes["href"].Value).Where(IsValidPostUrl).Distinct();
            var postLinks = new Collection<ParsedPostLinkModel>();

            foreach (var postUrl in postUrls) {
                postLinks.Add(new ParsedPostLinkModel {
                    PostUri = new Uri(baseUri, postUrl)
                });
            }

            return postLinks;
        }

        private IEnumerable<ParsedPostContentModel> GetPostContents(ICheckStrategy checkStrategy, HtmlDocument document) {
            var downloadUrls = document.DocumentNode.SelectNodes("//a[@href]")
                                       .Where(du => IsValidPostDownloadUrl(du.Attributes["href"].Value))
                                       .Select(pc => new ParsedPostContentModel {
                                           Title = pc.InnerText.Trim(),
                                           DownloadUri = new Uri(pc.Attributes["href"].Value),
                                       });
            downloadUrls = GetCandidatePostContents(downloadUrls).Where(pc => {
                var isDuplicate = checkStrategy.IsDuplicate(Path.GetFileName(HttpUtility.UrlDecode(pc.DownloadUri.AbsolutePath)));
                if (isDuplicate)
                    _foundedDuplicateItems++;
                return !isDuplicate;
            });

            return downloadUrls;
        }

        private void OnProgressReport(string title, Uri downloadUri) {
            if (ProgressReport != null) {
                lock (_lock) {
                    if (ProgressReport != null)
                        ProgressReport(this, new GrabberProgressReportEventArgs(title, downloadUri));
                }
            }
        }

        private void OnProgressChanged(int currentProgress) {
            if (ProgressChanged != null) {
                lock (_lock) {
                    if (ProgressChanged != null)
                        ProgressChanged(this, new GrabberProgressChangedEventArgs(currentProgress));
                }
            }
        }

        private void OnGrabbingFinished() {
            if (GrabbingFinished != null) {
                lock (_lock) {
                    if (GrabbingFinished != null)
                        GrabbingFinished(this, new EventArgs());
                }
            }
        }
    }
}