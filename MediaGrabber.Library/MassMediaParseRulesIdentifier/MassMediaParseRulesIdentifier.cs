using HtmlAgilityPack;
using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaGrabber.Library.MassMediaParseRulesIdentifier
{
    /// <summary>
    /// This identifier works with only one Mass Media
    /// </summary>
    public class MassMediaParseRulesIdentifier : IMassMediaParseRulesIdentifier
    {
        private MassMedia _massMedia;
        //private IEnumerable<MayBeArticlePage> _mayBeArticlePages;
        public MassMediaParseRulesIdentifier(MassMedia massMedia)
        {
            _massMedia = massMedia;
            //_mayBeArticlePages = new List<MayBeArticlePage>();
        }

        /// <summary>
        /// Tries to get rss pages from website.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RssPage> GetRssPages()
        {
            var rssPagefinder = new RssPageFinder(_massMedia);
            var rssPages = rssPagefinder.FindRssPages();
            return rssPages;
        }

        /// <summary>
        /// Gets articles whole html pages from rss data.
        /// </summary>
        /// <param name="rssPage"></param>
        /// <returns></returns>
        public override IEnumerable<MayBeArticlePage> GetArticlesFromRssPage(RssPage rssPage)
        {
            var rssReader = new RssReader(_massMedia);
            var articlesBasicData = rssReader.GetArticlesBasicDataFromRssPage(rssPage);
            return articlesBasicData.Select(x => new MayBeArticlePage(x.Url)
            {
                BodyHtml = x.BodyHtml
            });
        }

        /// <summary>
        /// Tries to find pages with articles using main page of mass media as a start point.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<MayBeArticlePage> GetArticlesFromMainPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to identify single parsing rule of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <returns></returns>
        public override ParsingRule GetMostProbableParsingRule()
        {
            ParsingRule result = null;
            var rssPages = GetRssPages();
            if(rssPages == null || !rssPages.Any())
            {
                var mayBeArticles = GetArticlesFromMainPage();
                result = ProcessHtmlWithArticlesToIdentifyRulesWithoutUsingRssPages().FirstOrDefault();
                //TODO Process rules - find most popular.
                //
                //
            }
            else
            {
                var rules = new List<ParsingRule>();
                foreach(var rssPage in rssPages)
                {
                    var mayBeArticles = GetArticlesFromRssPage(rssPage);
                    var rule = ProcessHtmlWithArticlesToIdentifyRulesUsingRssPages(mayBeArticles, rssPage)
                        .FirstOrDefault();
                    if (rule != null)
                        rules.Add(rule);

                    //TODO Process rules - find most popular.
                    //
                    //
                }
                // here we can use 'description' elements if there in rss
                // in order to identify article text on the page.
                
            }

            return result;
        }

        /// <summary>
        /// Tries to identify parsing rules of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <param name="maxParsingRulesNumber"></param>
        /// <returns></returns>
        public override IEnumerable<ParsingRule> GetMostProbableParsingRules(int maxParsingRulesNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules using RSS pages data.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        private IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPages(IEnumerable<MayBeArticlePage> articles, RssPage rssPage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules without using RSS pages data.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        private IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesWithoutUsingRssPages()
        {
            throw new NotImplementedException();
        }

        private HtmlNode FindHtmlNodeWithLongestText(MayBeArticlePage articlePage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string FindBestClassXPathForHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element id.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string FindBestIdXPathForHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            throw new NotImplementedException();
        }
    }
}
