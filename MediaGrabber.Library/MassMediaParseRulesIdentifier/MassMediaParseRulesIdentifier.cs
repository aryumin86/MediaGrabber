using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.MassMediaParseRulesIdentifier
{
    /// <summary>
    /// This identifier works with only one Mass Media
    /// </summary>
    public class MassMediaParseRulesIdentifier : IMassMediaParseRulesIdentifier
    {
        public MassMedia MassMedia { get; private set; }
        private IEnumerable<MayBeArticlePage> MayBeArticlePages { get; set; }
        public MassMediaParseRulesIdentifier(MassMedia massMedia)
        {
            MassMedia = massMedia;
        }

        public override IEnumerable<MassMediaRssPage> GetRssPages()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MayBeArticlePage> GetArticlesFromRssPage()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MayBeArticlePage> GetArticlesFromMainPage()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MayBeArticlePage> GetArticlesFromCategoriesPagesPage()
        {
            throw new NotImplementedException();
        }

        public override MayBeArticlePage GetMayBePage(string url)
        {
            throw new NotImplementedException();
        }

        public override ParsingRule GetMostProbableParsingRule(IEnumerable<MayBeArticlePage> mayBeArticlePages)
        {
            throw new NotImplementedException();
        }
    }
}
