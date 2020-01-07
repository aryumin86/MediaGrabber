using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.ParseRulesIdentifier
{
    public abstract class IMassMediaParseRulesIdentifier
    {
        /// <summary>
        /// Gets articles whole html pages from rss data.
        /// </summary>
        /// <param name="rssPage"></param>
        /// <returns></returns>
        public abstract IEnumerable<MayBeArticlePage> GetArticlesFromRssPage(RssPage rssPage);
        /// <summary>
        /// Tries to find pages with articles using main page of mass media as a start point.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<MayBeArticlePage> GetArticlesFromMainPage();
        /// <summary>
        /// Tries to identify single parsing rule of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <returns></returns>
        public abstract ParsingRule GetMostProbableParsingRule(IEnumerable<RssPage> rssPages);
        /// <summary>
        /// Tries to identify parsing rules of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <param name="maxParsingRulesNumber"></param>
        /// <returns></returns>
        public abstract IEnumerable<ParsingRule> GetMostProbableParsingRules(int maxParsingRulesNumber);
    }
}
