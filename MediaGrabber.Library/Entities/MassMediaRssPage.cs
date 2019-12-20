using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    public class MassMediaRssPage
    {
        public int Id { get; set; }
        public int MassMediaId { get; set; }
        public MassMedia MassMedia { get; set; }
        public DateTime WhenAdded { get; set; }
    }
}
