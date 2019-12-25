using System;
using System.Collections.Generic;
using System.Text;
using MediaGrabber.Library.Enums;

namespace MediaGrabber.Library.Entities
{
    public class MassMedia
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime WhenAdded { get; set; }
        public string MainUrl { get; set; }
        public IEnumerable<MassMediaRssPage> RssPages { get; set; }        
        public IEnumerable<ParsingRule> ParsingRules { get; set; }
        public string Encoding {get; set;}
        public WebSiteOpeningType WebSiteOpeningType {get; set;}
    }
}
