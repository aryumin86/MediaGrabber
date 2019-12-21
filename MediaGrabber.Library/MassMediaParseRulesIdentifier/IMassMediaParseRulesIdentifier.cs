using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.MassMediaParseRulesIdentifier
{
    public abstract class IMassMediaParseRulesIdentifier
    {
        public abstract IEnumerable<MassMediaRssPage> GetRssPages();
        public abstract IEnumerable<MayBeArticlePage> GetArticlesFromRssPage();
        public abstract IEnumerable<MayBeArticlePage> GetArticlesFromMainPage();
        public abstract IEnumerable<MayBeArticlePage> GetArticlesFromCategoriesPagesPage();
        public abstract MayBeArticlePage GetMayBePage(string url);
        public abstract ParsingRule GetMostProbableParsingRule(IEnumerable<MayBeArticlePage> mayBeArticlePages);
        
    }
}
