using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

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
            Url = url;
        }
        public string BodyHtml { get; set; }
        /// <summary>
        /// One of elements with these XPaths probably contains article text.
        /// Ideally it is an Id of tag.
        /// </summary>
        public SortedList<int, MayBeArticleContainer> MayBeArticleContainers { get; set; }
        public string ProbableArticleText { get; set; }
        public string ProbableArticleHtml { get; set; }
        public string ProbableBodyPart {get; set;}
        public DateTime WhenPublished {get; set;}
        public string Url {get; set;}
        public HtmlDocument HtmlDocument {get; private set;}

        public void LoadHtml(){
            if(!string.IsNullOrWhiteSpace(this.BodyHtml)){
                this.HtmlDocument = new HtmlDocument();
                this.HtmlDocument.LoadHtml(this.BodyHtml);
            }
            else
                throw new InvalidOperationException("Can't load html since there is empty of null BodyHtml property");
        }
    }
}
