using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    /// <summary>
    /// Article in mass media.
    /// </summary>
    public class Article
    {
        public int Id { get; set; }
        public string BodyHtml { get; set; }
        public string BodyText { get; set; }
        /// <summary>
        /// Part of text - e.g.static text description from RSS.
        /// </summary>
        /// <value></value>
        public string BodyPart {get; set;}
        public string Title { get; set; }
        public string MassMediaId { get; set; }
        public string Url { get; set; }
        public DateTime WhenGrabbed { get; set; }
        public DateTime WhenPublished { get; set; }
    }
}
