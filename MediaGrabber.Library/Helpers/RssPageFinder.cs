using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using HtmlAgilityPack;
using System.Net;
using System.Net.Http;

namespace MediaGrabber.Library.Helpers
{
    public class RssPageFinder : IRssPageFinder
    {
        private MassMedia _massMedia;
        public RssPageFinder(MassMedia massMedia){
            _massMedia = massMedia;
        }

        public IEnumerable<RssPage> FindRssPages(string baseUrl)
        {
            var rssPages = new List<RssPage>();
            // opens main page
            var basePageHtml = this.GetPageHtml(baseUrl).Result;
            // gets all page links
            var basePageLinks = this.GetAllLinks(basePageHtml);
            // Links that probably direct to rss pages
            var mayBeRssPageLinks = basePageLinks.Where(l => MayBeLinkToRssPage(l));
            // opens each rss page link with pauses and check each as a valid rss page.
            // If it is a valid rss page - adds it to result list.
            foreach(var l in mayBeRssPageLinks){
                var pageHtml = GetPageHtml(l).Result;
                if(IsValidRssPage(pageHtml))
                    rssPages.Add(new RssPage{ Url = l, MassMediaId = _massMedia.Id });
                // If it is not a valid rss page - gets all link on this page 
                // and repead alorythm for them
                else{
                    var pageLinks = this.GetAllLinks(pageHtml);
                    var mayBeRssPageLinks2 = pageLinks.Where(l => MayBeLinkToRssPage(l));
                    foreach(var l2 in mayBeRssPageLinks2){
                        var pageHtml2 = GetPageHtml(l2).Result;
                        if(IsValidRssPage(pageHtml2)){
                            rssPages.Add(new RssPage{ Url = l2, MassMediaId = _massMedia.Id });
                        }
                        Thread.Sleep(_massMedia.ParsingPauseInMs);
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
        public async Task<string> GetPageHtml(string url, 
            WebSiteOpeningType webSiteOpeningType = WebSiteOpeningType.HtmlAgilityPack)
        {
            if(webSiteOpeningType == WebSiteOpeningType.HtmlAgilityPack){
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            else{
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> ParsePageforMayBeRssUrls(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var linksELems = doc.DocumentNode.SelectNodes("//a[@href]");
            var linksUrls = linksELems.Select(e => e.GetAttributeValue("href", null));
            var rssLinks = linksUrls.Where(link => this.MayBeLinkToRssPage(link));
            var absoluteRssLinks = rssLinks.Select(l => CreateAbsoluteUrlIfRelative(l));
            return absoluteRssLinks;
        }
        
        public bool IsValidRssPage(string html){
            throw new NotImplementedException();
        }

        private string[] GetAllLinks(string html){
            throw new NotImplementedException();
        }

        public bool MayBeLinkToRssPage(string link){
            if(link.Contains("rss") || link.Contains("feed"))
                return true;
            else
                return false;
        }

        private string CreateAbsoluteUrlIfRelative(string link){
            Uri uri;
            if(!Uri.TryCreate(link, UriKind.Absolute, out uri))
                uri = new Uri(new Uri(this._massMedia.MainUrl),link);

            return uri.AbsoluteUri;
        }
    }
}
