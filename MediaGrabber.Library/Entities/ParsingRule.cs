using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Entities
{
    public class ParsingRule
    {
        public int Id { get; set; }
        public DateTime WhenAdded { get; set; }
        public string XPath { get; set; }
        public bool ManuallyChecked { get; set; } = false;
    }
}
