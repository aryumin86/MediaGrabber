using HtmlAgilityPack;
using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using MediaGrabber.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaGrabber.Library.ParseRulesIdentifier
{
    /// <summary>
    /// This identifier works with only one Mass Media
    /// </summary>
    public class MassMediaParseRulesIdentifier : IMassMediaParseRulesIdentifier
    {
        private MassMedia _massMedia;
        private readonly int _minimumArticlesNumberWithDescriptionFromRss = 3;
        public MassMediaParseRulesIdentifier(MassMedia massMedia)
        {
            _massMedia = massMedia;
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
                BodyHtml = x.BodyHtml,
                ProbableBodyPart = x.BodyPart
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
        public override ParsingRule GetMostProbableParsingRule(IEnumerable<RssPage> rssPages)
        {
            ParsingRule result = null;
            IEnumerable<MayBeArticlePage> mayBeArticles = null;
            IEnumerable<ParsingRule> rules = null;
            if(rssPages != null && rssPages.Any()){
                
                foreach(var rssPage in rssPages)
                {
                    IEnumerable<ParsingRule> rulesFromRssPageArticles = null;
                    mayBeArticles = GetArticlesFromRssPage(rssPage);
                    if(mayBeArticles.Where(a => !string.IsNullOrWhiteSpace(a.ProbableBodyPart)).Count() < _minimumArticlesNumberWithDescriptionFromRss)
                    {
                        rulesFromRssPageArticles = ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithoutDescriptions(rssPage, mayBeArticles);
                    }
                    else
                    {
                        rulesFromRssPageArticles = 
                            ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(rssPage, mayBeArticles);
                    }
                    
                    if(rules == null)
                        rules = new List<ParsingRule>();
                    if(rulesFromRssPageArticles != null && rulesFromRssPageArticles.Any())
                        (rules as List<ParsingRule>).AddRange(rulesFromRssPageArticles);
                    // may be it is a good idea here to try to 
                    // identify parsing rule for THIS rss page (it is foreach for all rss pages)
                }

                //TODO Process rules - find most popular.
                //
                //
                return result;
            }

            mayBeArticles = GetArticlesFromMainPage();
            rules = ProcessHtmlWithArticlesToIdentifyRulesNotUsingRssPages(mayBeArticles);
            //TODO Process rules - find most popular.
            //
            //
            return rules.FirstOrDefault();
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
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(RssPage rssPage, IEnumerable<MayBeArticlePage> articles)
        {
            IEnumerable<ParsingRule> result = null;

            // Filtering articles: getting MayBeArticlePage objects collected 
            // from RSS pages which has a description tag
            articles = articles.Where(a => !string.IsNullOrWhiteSpace(a.ProbableBodyPart));

            // If there are at least 'minArticlesWithDescriptin' such articles we use them for 
            // identifying containers which contain article text on article page
            if (articles.Count() < _minimumArticlesNumberWithDescriptionFromRss)
                return result;



            // Articles with containers are opened


            return result;
        }

        

        /// <summary>
        /// Process articles to identify parsing rules without using RSS pages data.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesNotUsingRssPages(IEnumerable<MayBeArticlePage> articles)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules using RSS pages without description.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithoutDescriptions(RssPage rssPage, IEnumerable<MayBeArticlePage> articles)
        {
            throw new NotImplementedException();
        }

        private HtmlNode FindHtmlNodeWithLongestText(MayBeArticlePage articlePage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens article's url.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private string OpenArticleUrl(string url, WebSiteOpeningType webSiteOpeningType = WebSiteOpeningType.HtmlAgilityPack)
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
