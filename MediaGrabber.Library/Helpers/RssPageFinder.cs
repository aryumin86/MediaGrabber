using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Helpers
{
    public class RssPageFinder : IRssPageFinder
    {
        public IEnumerable<RssPage> FindRssPages(string baseUrl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets page html.
        /// TODO: Should work with htmlAgilityPack and Puppertier according to the source.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetPageHtml(string url, WebSiteOpeningType webSiteOpeningType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParsePageforRssUrls(string html)
        {
            throw new NotImplementedException();
        }
    }
}
