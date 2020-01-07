using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    /// <summary>
    /// Some page at mass media website.
    /// Needed for parsing rules identification.
    /// </summary>
    public class MayBeArticlePage
    {
        public MayBeArticlePage(string url)
        {
            _url = url;
        }
        /// <summary>
        /// Url of this page which probably contains article.
        /// </summary>
        private readonly string _url;
        public string BodyHtml { get; set; }
        /// <summary>
        /// One of elements with these XPaths probably contains article text.
        /// Ideally it is an Id of tag.
        /// </summary>
        public SortedList<int, MayBeArticleContainer> MayBeArticleContainers { get; set; }
        public string ProbableArticleText { get; set; }
        public string ProbableArticleHtml { get; set; }
        public string ProbableBodyPart {get; set;}

        /// <summary>
        /// Looking for most probable article text containers.
        /// </summary>
        public void FindProbableArticleContainers(int maxElems)
        {
            MayBeArticleContainers = new SortedList<int, MayBeArticleContainer>(maxElems);
        }
    }
}
