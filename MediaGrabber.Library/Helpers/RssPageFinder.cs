using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace MediaGrabber.Library.Helpers
{
    public class RssPageFinder : IRssPageFinder
    {
        public MassMedia MassMedia { get; private set; }
        public RssPageFinder(MassMedia massMedia){
            this.MassMedia = massMedia;
        }

        public IEnumerable<RssPage> FindRssPages(string baseUrl)
        {
            var rssPages = new List<RssPage>();
            // opens main page
            var basePageHtml = this.GetPageHtml(baseUrl);
            // gets all page links
            var basePageLinks = this.GetAllLinks(basePageHtml);
            // Links that probably direct to rss pages
            var mayBeRssPageLinks = basePageLinks.Where(l => MayBeLinkToRssPage(l));
            // opens each rss page link with pauses and check each as a valid rss page.
            // If it is a valid rss page - adds it to result list.
            foreach(var l in mayBeRssPageLinks){
                var pageHtml = GetPageHtml(l);
                if(IsValidRssPage(pageHtml))
                    rssPages.Add(new RssPage{ Url = l, MassMediaId = this.MassMedia.Id });
                // If it is not a valid rss page - gets all link on this page 
                // and repead alorythm for them
                else{
                    var pageLinks = this.GetAllLinks(pageHtml);
                    var mayBeRssPageLinks2 = pageLinks.Where(l => MayBeLinkToRssPage(l));
                    foreach(var l2 in mayBeRssPageLinks2){
                        var pageHtml2 = GetPageHtml(l2);
                        if(IsValidRssPage(pageHtml2)){
                            rssPages.Add(new RssPage{ Url = l2, MassMediaId = this.MassMedia.Id });
                        }
                        Thread.Sleep(this.MassMedia.ParsingPauseInMs);
                    }
                }
            }

            // filter rss page doubles
            rssPages = rssPages
                .GroupBy(p => new {p.Url, p} )
                .Select(p => p.FirstOrDefault())
                .ToList();
            return rssPages;
        }

        /// <summary>
        /// Opens link ans gets page html.
        /// TODO: Should work with htmlAgilityPack and Puppertier according to the source.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetPageHtml(string url, 
            WebSiteOpeningType webSiteOpeningType = WebSiteOpeningType.HtmlAgilityPack)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ParsePageforRssUrls(string html)
        {
            throw new NotImplementedException();
        }
        
        public bool IsValidRssPage(string html){
            throw new NotImplementedException();
        }

        private string[] GetAllLinks(string html){
            throw new NotImplementedException();
        }

        public bool MayBeLinkToRssPage(string link){
            throw new NotImplementedException();
        }
    }
}
