using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    public class MassMediaRssPage
    {
        public int Id { get; set; }
        public string Url {get; set;}
        public int MassMediaId { get; set; }
        public DateTime WhenAdded { get; set; }
    }
}
