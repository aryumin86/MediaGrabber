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
        private readonly MassMedia _massMedia;
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
            string basePageHtml = string.Empty;
            try
            {
                basePageHtml = this.GetPageHtml(this._massMedia.MainUrl).Result;
            }
            catch(Exception ex)
            {
                this._massMedia.ParsingRuleIdentificationState = ParsingRuleIdentificationState.Failed;
                return null;
            }

            // gets all page links
            var mayBeBasePageRssLinks = ParsePageforMayBeRssUrls(basePageHtml);
            // change links protocol if needed
            mayBeBasePageRssLinks = mayBeBasePageRssLinks
                .Where(l => NormalLinkToSomePage(l))
                .Where(l => IsLocalLink(l))
                .Select(l => CreateAbsoluteUrlIfRelative(l))
                .Select(l => ChangeProtokolIfNeeded(l));

            // opens each rss page link with pauses and check each for being a valid rss page.
            // If it is a valid rss page - adds it to result list.
            foreach (var l in mayBeBasePageRssLinks)
            {
                try
                {
                    var pageHtml = GetPageHtml(l).Result;
                    if (string.IsNullOrWhiteSpace(pageHtml))
                        continue;

                    if (IsValidRssPage(pageHtml))
                        rssPages.Add(new RssPage { Url = l, MassMediaId = _massMedia.Id });
                    // If it is not a valid rss page - gets all link on this page 
                    // and repead alorythm for them
                    else
                    {
                        var mayBeRssLinks = ParsePageforMayBeRssUrls(pageHtml);
                        var mayBeRssPageLinks2 = mayBeRssLinks
                            .Where(x => NormalLinkToSomePage(x))
                            .Where(x => IsLocalLink(x))
                            .Select(x => CreateAbsoluteUrlIfRelative(x))
                            .Select(x => ChangeProtokolIfNeeded(x));

                        foreach (var l2 in mayBeRssPageLinks2)
                        {
                            var pageHtml2 = GetPageHtml(l2).Result;
                            if (IsValidRssPage(pageHtml2))
                            {
                                rssPages.Add(new RssPage { Url = l2, MassMediaId = _massMedia.Id });
                            }
                            Thread.Sleep(_massMedia.ParsingPauseInMs);
                        }
                    }
                }
                catch(Exception ex)
                {
                    
                }                
            }

            // filter rss page doubles
            rssPages = rssPages
                .GroupBy(p => p.Url )
                .Select(p => p.FirstOrDefault())
                .ToList();
            return rssPages;
        }

        /// <summary>
        /// Filtering links if they target to main page or to the same page.
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private bool NormalLinkToSomePage(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return false;
            if (link == "/" || link == "#")
                return false;

            return true;
        }

        /// <summary>
        /// Changes link protocol if it is needed for correct http request.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private string ChangeProtokolIfNeeded(string url)
        {
            string res = null;
            Uri uri;
            if(Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                if (uri.Scheme.ToUpperInvariant() != "HTTP" && uri.Scheme.ToUpperInvariant() != "HTTPS")
                {
                    res = this._massMedia.Uri.Scheme + "://" + this._massMedia.Uri.Host + uri.PathAndQuery;
                }
                else
                    res = url;
            }

            return res;
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
            string res = string.Empty;

            if(webSiteOpeningType == WebSiteOpeningType.HtmlAgilityPack){
                using (var client = new HttpClient())
                {
                    using(var response = await client.GetAsync(url).ConfigureAwait(false))
                    {
                        if(response.StatusCode == HttpStatusCode.Moved)
                        {
                            var redirectUri = response.Headers.Location;
                            if (!redirectUri.IsAbsoluteUri)
                            {
                                redirectUri = new Uri(this._massMedia.MainUrl + redirectUri);
                                
                            }
                            res = GetPageHtml(redirectUri.AbsoluteUri).Result;
                        }
                        else
                        {
                            using (var content = response.Content)
                            {
                                res = await content.ReadAsStringAsync().ConfigureAwait(false);
                            }
                        }                        
                    }
                }
            }
            else{
                throw new NotImplementedException();
            }

            return res;
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
            var rssLinks2 = 
                linksELems.Where(e => this.MayBeLinkToRssPageByLinkText(e.InnerText))
                .Select(e => e.GetAttributeValue("href", null));
            rssLinks = rssLinks.Union(rssLinks2);
            var absoluteRssLinks = rssLinks.Select(l => CreateAbsoluteUrlIfRelative(l));
            return absoluteRssLinks;
        }
        
        /// <summary>
        /// Checks if the page has valid rss format.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public bool IsValidRssPage(string html){
            if (string.IsNullOrWhiteSpace(html))
                return false;

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
        /// Identifies if link directs to the same site or subdomain.
        /// </summary>
        /// <returns></returns>
        public bool IsLocalLink(string link)
        {
            var res = true;
            Uri uri;
            if (Uri.TryCreate(link, UriKind.Absolute, out uri))
            {
                if (uri.Host != this._massMedia.Uri.Host && !IsSubDomain(_massMedia.Uri, uri))
                    res = false;
            }

            return res;
        }

        /// <summary>
        /// Checks if a link is a subdomain of another link.
        /// </summary>
        /// <param name="mainDomain"></param>
        /// <param name="mayBeSubDomain"></param>
        /// <returns></returns>
        private bool IsSubDomain(Uri mainDomainLink, Uri mayBeSubDomainLink)
        {
            var nodes = mayBeSubDomainLink.Host.Split('.')
                .Where(n => n.ToUpperInvariant() != "WWW")
                .ToArray();
            int startNode = 0;

            if (mainDomainLink.Host.Split('.')
                .Where(n => n.ToUpperInvariant() != "WWW")
                .ToArray()[0 + startNode].ToUpperInvariant() == nodes[startNode + 1].ToUpperInvariant())
                return true;

            return false;
        }
    }
}
