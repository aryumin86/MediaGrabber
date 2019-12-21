using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    /// <summary>
    /// This class representds one of probable article's text containers.
    /// </summary>
    public class MayBeArticleContainer
    {
        public string XPath { get; set; }
        public string ArticleText { get; set; }
        public string ArticleHtml { get; set; }
        public IEnumerable<string> ContainerClasses { get; set; }
        public int? ContainerId { get; set; }
    }
}
