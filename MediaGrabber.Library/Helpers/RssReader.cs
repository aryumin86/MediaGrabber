using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Reads RSS xml and gets all articles data from it.
    /// </summary>
    public class RssReader : IRssReader
    {
        private MassMedia _massMedia;

        public RssReader(MassMedia massMedia) 
        {
            _massMedia = massMedia;
        }

        /// <summary>
        /// Returns all articles links from RSS page.
        /// </summary>
        /// <param name="rssPage"></param>
        /// <returns></returns>
        public IEnumerable<Article> GetArticlesBasicDataFromRssPage(RssPage rssPage)
        {
            if(!IsValidRssPage(rssPage.XmlContent)){
                throw new FormatException("loaded rss page has invalid format");
            }
            else{
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Does xml has valid format as RSS page.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public bool IsValidRssPage(string xml)
        {
            if(string.IsNullOrWhiteSpace(xml)){
                return false;
            }
                
            var res = false;

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                var rootName = xmlDoc.DocumentElement.Name;
                if(rootName.ToUpperInvariant() == "RSS")
                     res = true;
            }
            catch(Exception ex)
            {
                
            }

            return res;
        }
    }
}
