using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MediaGrabber.Library.MMParseRulesIdentifier;
using MediaGrabber.Library.Helpers;

namespace MediaGrabber.Library.Tests.MassMediaParseRulesIdentifier
{
    public class MassMediaParseRulesIdentifierTests
    {
        [Theory]
        [InlineData("RssPage_v2.0_2.xml","","","")] // TODO Change to real data
        [Trait("Category", "Unit")]
        public void GetMostProbableParsingRuleSuccess_UsingRssWithArticlesDescriptionsTags(string rssPageXml, string articleHtml1, string articleHtml2, string articleHtml3)
        {
            var massMedia = new MassMedia("http://aa.ru")
            {
                Encoding = "utf8",
                Id = 1
            };

            var rulesIdentifier = 
                new MediaGrabber.Library.MMParseRulesIdentifier.MassMediaParseRulesIdentifier(massMedia);
            var rssReader = new RssReader(massMedia);

            var rssPage = new RssPage()
            {
                XmlContent = rssPageXml
            };

            var articlesWithBasicData = rssReader.GetArticlesBasicDataFromRssPage(rssPage);
            rulesIdentifier.GetMostProbableParsingRule();
        }

        [Theory]
        [InlineData("RssPage_v2.0_2.xml", "", "", "")] // TODO Change to real data
        [Trait("Category", "Unit")]
        public void GetMostProbableParsingRuleSuccess_UsingRssWithoutArticlesDescriptionsTags(string rssPageXml, string articleHtml1, string articleHtml2, string articleHtml3)
        {

        }

        [Theory]
        [InlineData("RssPage_v2.0_2.xml", "", "")] // TODO Change to real data
        [Trait("Category", "Unit")]
        public void GetMostProbableParsingRuleSuccess_NotUsingRss(string articleHtml1, string articleHtml2, string articleHtml3)
        {

        }
    }
}
