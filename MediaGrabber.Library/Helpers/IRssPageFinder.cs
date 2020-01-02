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
        /// <summary>
        /// Main method for looking for rss pages at the website.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>        
        IEnumerable<RssPage> FindRssPages();

        /// <summary>
        /// Opens link ans gets page html.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="webSiteOpeningType"></param>
        /// <returns></returns>
        Task<string> GetPageHtml(string url, WebSiteOpeningType webSiteOpeningType);

        /// <summary>
        /// Tries to find links to pages that could be RSS pages.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        IEnumerable<string> ParsePageforMayBeRssUrls(string html);

        /// <summary>
        /// Checks if the page has valid rss format.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        bool IsValidRssPage(string html);
    }
}
