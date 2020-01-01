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
using System.Xml;
using System.Globalization;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Tries to identify rss urls on website.
    /// </summary>
    public class RssPageFinder : IRssPageFinder
    {
        private MassMedia _massMedia;
        public RssPageFinder(MassMedia massMedia){
            _massMedia = massMedia;
        }

        /// <summary>
        /// Main method for looking for rss pages at the website.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public IEnumerable<RssPage> FindRssPages()
        {
            var rssPages = new List<RssPage>();
            // opens main page
            var basePageHtml = this.GetPageHtml(this._massMedia.MainUrl).Result;
            // gets all page links
            var basePageLinks = this.GetAllLinks(basePageHtml);
            // Links that probably direct to rss pages
            var mayBeRssPageLinks = basePageLinks.Where(l => MayBeLinkToRssPageByLinkFormat(l));
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
                    var mayBeRssPageLinks2 = pageLinks.Where(l => MayBeLinkToRssPageByLinkFormat(l));
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
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(new Uri(url)).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return responseBody;
                }                
            }
            else{
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Tries to find links to pages that could be RSS pages.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public IEnumerable<string> ParsePageforMayBeRssUrls(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var linksELems = doc.DocumentNode.SelectNodes("//a[@href]");
            var linksUrls = linksELems.Select(e => e.GetAttributeValue("href", null));
            var rssLinks = linksUrls.Where(link => this.MayBeLinkToRssPageByLinkFormat(link));
            //TODO add MayBeLinkToRssPageByLinkText selection condition
            var absoluteRssLinks = rssLinks.Select(l => CreateAbsoluteUrlIfRelative(l));
            return absoluteRssLinks;
        }
        
        /// <summary>
        /// Checks if the page has valid rss format.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public bool IsValidRssPage(string html){
            var res = false;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(html);
                var rootName = xmlDoc.DocumentElement.Name;
                if(rootName.ToUpperInvariant() == "RSS")
                     res = true;
            }
            catch(Exception ex)
            {
                //TODO log info if it is not valid format
            }

            return res;
        }

        /// <summary>
        /// Gets all RSS links.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private IEnumerable<string> GetAllLinks(string html){
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var linksELems = doc.DocumentNode.SelectNodes("//a[@href]");
            var linksUrls = linksELems.Select(e => e.GetAttributeValue("href", null));
            return linksUrls;
        }

        /// <summary>
        /// According to link's text identify if it could be a link to rss page.
        /// </summary>
        /// <param name="linkAddress"></param>
        /// <returns></returns>
        private bool MayBeLinkToRssPageByLinkFormat(string linkAddress){
            if(linkAddress.ToUpperInvariant().Contains("RSS", StringComparison.InvariantCultureIgnoreCase) 
                || linkAddress.ToUpperInvariant().Contains("FEED", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

        private bool MayBeLinkToRssPageByLinkText(string linkText)
        {
            if (linkText.ToUpperInvariant().Contains("RSS", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Creates absolute link from relative if link is relative. 
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private string CreateAbsoluteUrlIfRelative(string link){
            Uri uri;
            if(!Uri.TryCreate(link, UriKind.Absolute, out uri))
                uri = new Uri(new Uri(this._massMedia.MainUrl), link);

            return uri.AbsoluteUri;
        }

        /// <summary>
        /// Identifies if link directs to the same site.
        /// </summary>
        /// <returns></returns>
        private bool IsLocalLink(string link, string massMediaUrl)
        {
            var res = true;
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                if (uri.Host != new Uri(massMediaUrl, UriKind.Absolute).Host)
                    res = false;
            }

            return res;
        }
    }
}
