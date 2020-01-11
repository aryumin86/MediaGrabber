using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MediaGrabber.Library.ParseRulesIdentifier;
using MediaGrabber.Library.Helpers;
using System.IO;
using System.Linq;
using System.Threading;

namespace MediaGrabber.Library.Tests.ParseRulesIdentifier
{
    public class MassMediaParseRulesIdentifierTests
    {
        /// <summary>
        /// Key is article html 1 - like 1.html (1)
        /// Value is article url.
        /// </summary>
        private Dictionary<int, string> articlesIdsAndUrls = new Dictionary<int, string>();

        /// <summary>
        /// Filling test folder data with articles html and rss that has links to these articles.
        /// </summary>
        public MassMediaParseRulesIdentifierTests()
        {
            var rssXmlurl = "https://andrey.moveax.ru/category/feed/%D0%92%D0%BE%D0%B7%D0%BC%D0%BE%D0%B6%D0%BD%D0%BE%D1%81%D1%82%D0%B8-C";

            var binPath = Environment.CurrentDirectory;
            string xmlPageFilePath =
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, 
                "TestsData", "ArticlesHtmlAndRssPages", "RssPageHasDescriptionTags", "rssPage.xml");
            
            if (File.Exists(xmlPageFilePath))
            {
                //clearing the folder with test data
                string dirPath = Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName,
                    "TestsData", "ArticlesHtmlAndRssPages", "RssPageHasDescriptionTags");
                var di = new DirectoryInfo(dirPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            var massMedia = new MassMedia("https://andrey.moveax.ru");
            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPageReader = new RssReader(massMedia);
            var rssPageXml = rssPageFinder.GetPageHtml(rssXmlurl).Result;
            using (FileStream fs = File.Create(xmlPageFilePath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(rssPageXml);
                fs.Write(info, 0, info.Length);
            }

            var rssPage = new RssPage() { XmlContent = rssPageXml };
            var i = 1;
            var links = rssPageReader.GetArticlesBasicDataFromRssPage(rssPage).ToList();
            links.ForEach(a =>
                {
                    var articleFilePath = Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName,
            "TestsData", "ArticlesHtmlAndRssPages", "RssPageHasDescriptionTags", $"{i}.html");
                    var articleHtml = rssPageFinder.GetPageHtml(a.Url).Result;

                    using (FileStream fs = File.Create(articleFilePath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(articleHtml);
                        fs.Write(info, 0, info.Length);
                    }

                    articlesIdsAndUrls.Add(i, a.Url);
                    i++;
                    Thread.Sleep(500);
                });
            
        }

        [Theory]
        [InlineData("rssPage.xml")]
        [Trait("Category", "Unit")]
        public void GetMostProbableParsingRuleSuccess_UsingRssWithArticlesDescriptionsTags(string rssPageXmlPath)
        {
            var massMedia = new MassMedia("http://aa.ru")
            {
                Encoding = "utf8",
                Id = 1
            };

            var binPath = Environment.CurrentDirectory;
            string xmlPageFilePath =
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName,
                "TestsData", "ArticlesHtmlAndRssPages", "RssPageHasDescriptionTags", rssPageXmlPath);

            var rssPageXml = File.ReadAllText(xmlPageFilePath);
            var articlesHtmlFiles = Directory.EnumerateFiles(Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName,
                "TestsData", "ArticlesHtmlAndRssPages", "RssPageHasDescriptionTags"));
            IEnumerable<MayBeArticlePage> mayBeArticles = articlesHtmlFiles
                .Where(a => !a.EndsWith("rssPage.xml"))
                .Select(a =>
            {
                var articleTestId = int.Parse(a.Split('\\').Last().Replace(".html", ""));
                return new MayBeArticlePage(articlesIdsAndUrls[articleTestId])
                {
                    BodyHtml = File.ReadAllText(a)
                };
            }).ToList();

            var rulesIdentifier = 
                new MediaGrabber.Library.ParseRulesIdentifier.MassMediaParseRulesIdentifier(massMedia);
            var rssReader = new RssReader(massMedia);

            var rssPage = new RssPage()
            {
                XmlContent = rssPageXml
            };
            
            var rule = rulesIdentifier.ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(rssPage, mayBeArticles);

            Assert.NotNull(rule);
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
