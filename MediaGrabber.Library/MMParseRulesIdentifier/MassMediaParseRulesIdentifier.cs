using HtmlAgilityPack;
using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaGrabber.Library.MMParseRulesIdentifier
{
    /// <summary>
    /// This identifier works with only one Mass Media
    /// </summary>
    public class MassMediaParseRulesIdentifier : IMassMediaParseRulesIdentifier
    {
        private MassMedia _massMedia;
        public MassMediaParseRulesIdentifier(MassMedia massMedia)
        {
            _massMedia = massMedia;
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
                result = ProcessHtmlWithArticlesToIdentifyRulesWithoutUsingRssPagesWithDescriptions().FirstOrDefault();
                //TODO Process rules - find most popular.
                //
                //

                if (result != null)
                    return result;
            }

            // if first way using RSS pages had no success:
            var rules = new List<ParsingRule>();
            foreach(var rssPage in rssPages)
            {
                var mayBeArticles = GetArticlesFromRssPage(rssPage);
                var rule = ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(mayBeArticles, rssPage)
                    .FirstOrDefault();
                if (rule != null)
                    rules.Add(rule);

                //TODO Process rules - find most popular.
                //
                //
            }
            // here we can use 'description' elements if there in rss
            // in order to identify article text on the page.
                
            

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
        /// Process articles to identify parsing rules using RSS pages data where
        /// RSS item elements have description elements.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        private IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(IEnumerable<MayBeArticlePage> articles, RssPage rssPage, int minArticlesWithDescriptin = 3)
        {
            IEnumerable<ParsingRule> result = null;

            // Filtering artucles: getting MayBeArticlePage objects collected 
            // from RSS pages which has a description tag

            // If there are at least 'minArticlesWithDescriptin' such articles we use them for 
            // identifying containers which contain article text on article page

            // Otherwise if there are less than 'minArticlesWithDescriptin' articles 
            // that have description from RSS
            // other method should be used: null will be returned;

            // Articles with containers are opened

            return result;
        }

        /// <summary>
        /// Opens article's url.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string OpenArticleUrl(string html)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules without using RSS pages data.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        private IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesWithoutUsingRssPagesWithDescriptions()
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

        /// <summary>
        /// Tries to find node of html element where certain text is presented.
        /// It should be unique text coincidences across all nodes.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        private HtmlNode FindNodeWithText(string text, string html)
        {
            throw new NotImplementedException();
        }
    }
}
