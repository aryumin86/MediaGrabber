using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

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
            var result = new List<Article>();

            if(!IsValidRssPage(rssPage.XmlContent)){
                throw new FormatException("loaded rss page has invalid format");
            }
            else{
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rssPage.XmlContent);
                var rssVersion = xmlDoc.DocumentElement.GetAttribute("version");
                switch(rssVersion){
                    case "0.91":
                    rssPage.RssVersion = Enums.RssVersion.V0dot91;
                        break;
                    case "0.92": 
                    rssPage.RssVersion = Enums.RssVersion.V0dot92;
                        break;
                    case "2.0":
                    rssPage.RssVersion = Enums.RssVersion.V2dot0;
                        break;
                }
                var elems = xmlDoc.DocumentElement.GetElementsByTagName("item");
                foreach(XmlElement elem in elems){
                    Article a = null;
                    string link = null;
                    string title = null;
                    DateTime pubDate = DateTime.MinValue;
                    string description = null;

                    if(rssPage.RssVersion == Enums.RssVersion.V0dot92){
                        throw new NotImplementedException();
                    }
                    else{ //v2.0 or v1.91
                        foreach(var n in elem.ChildNodes){
                            var node = n as XmlNode;
                            switch(node.Name){
                                case "title":
                                    title = node.InnerText;
                                    break;
                                case "link":
                                    link = node.InnerText;
                                    break;
                                case "description":
                                    description = node.InnerText;
                                    break;
                                case "pubDate":
                                    pubDate = DateTime.Parse(node.InnerText);
                                    break;
                            }
                        }

                        if(link != null){
                            a = new Article(){
                                Url = link,
                                Title = title,
                                BodyPart = description,
                                WhenPublished = pubDate
                            };
                            result.Add(a);
                        }
                    }
                }
            }

            return result;
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
