using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Tries to identify rss urls on website.
    /// </summary>
    public interface IRssPageFinder
    {
        public IEnumerable<RssPage> FindRssPages(string baseUrl);
        public Task<string> GetPageHtml(string url, WebSiteOpeningType webSiteOpeningType);
        public IEnumerable<string> ParsePageforMayBeRssUrls(string html);
    }
}
