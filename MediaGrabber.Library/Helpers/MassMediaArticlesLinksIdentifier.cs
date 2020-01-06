using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Tries to find links to articles starting from the main page of mass media.
    /// </summary>
    public class MassMediaArticlesLinksIdentifier : IMassMediaArticlesLinksIdentifier
    {
        private static string[] articlesCommonCategoriesNames = new string[] {
            "culture",
            "sport",
            "politic",
            "law",
            "econom",
            "world",
            "crime",
            "education"
        };

        public IEnumerable<string> GetArticlesLinks(MassMedia massMedia)
        {
            throw new NotImplementedException();
        }
    }
}
