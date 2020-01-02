using MediaGrabber.Library.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    public class RssPage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int MassMediaId { get; set; }
        public RssVersion RssVersion { get; set; }
        public string XmlContent {get; set;}
    }
}
