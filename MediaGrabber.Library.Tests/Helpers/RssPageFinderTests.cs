using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace MediaGrabber.Library.Tests.Helpers
{
    public class RssPageFinderTests
    {
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("dni.ru.html")]
        [InlineData("rg.ru.html")]
        public void ShouldFindRssPagesOnLocalFiles(string htmlFilePath)
        {
            var massMedia = new MassMedia();
            var binPath = Environment.CurrentDirectory;
            htmlFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "HtmlWithRssLinks", htmlFilePath);
            var pageHtml = File.ReadAllText(htmlFilePath);
            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPages = rssPageFinder.ParsePageforMayBeRssUrls(pageHtml);
            Assert.True(rssPages.Count() > 0);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("htmlWithoutRssUrls_1.html")]
        public void ShouldNotFindRssPagesOnLocalFiles(string htmlFilePath)
        {
            var massMedia = new MassMedia();
            var binPath = Environment.CurrentDirectory;
            htmlFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "HtmlWithoutRssLinks", htmlFilePath);
            var pageHtml = File.ReadAllText(htmlFilePath);
            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPages = rssPageFinder.ParsePageforMayBeRssUrls(pageHtml);
            Assert.True(rssPages.Count() == 0);
        }

        [Theory]
        [Trait("Category", "WebData")]
        [InlineData("abc.ru.html")]
        public void ShouldFindRssPagesOnRemoteWebSite(string url)
        {
            var massMedia = new MassMedia()
            {
                MainUrl = url,
                Encoding = "utf8",
                Id = 1
            };

            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPages = rssPageFinder.FindRssPages();
            Assert.True(rssPages.Count() > 0);
        }

        [Theory]
        [Trait("Category", "WebData")]
        [InlineData("ccc.ru.html")]
        public void ShouldNotFindRssPagesOnRemoteWebSite(string url)
        {

        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("RssPage_v0.91.xml")]
        [InlineData("RssPage_v0.92.xml")]
        [InlineData("RssPage_v2.0.xml")]
        [InlineData("RssPage_v2.0_2.xml")]
        public void ShoudIdentifyPageAsValidRss(string htmlFilePath)
        {
            var massMedia = new MassMedia();
            var binPath = Environment.CurrentDirectory;
            htmlFilePath =
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "ValidRssPages", htmlFilePath);
            var pageHtml = File.ReadAllText(htmlFilePath);
            var rssPageFinder = new RssPageFinder(massMedia);
            var valid = rssPageFinder.IsValidRssPage(pageHtml);
            Assert.True(valid);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("inv_1.html")]
        [InlineData("inv_2.xml")]
        public void ShoudNotIdentifyPageAsValidRss(string htmlFilePath)
        {
            var massMedia = new MassMedia();
            var binPath = Environment.CurrentDirectory;
            htmlFilePath =
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "InvalidRssPages", htmlFilePath);
            var pageHtml = File.ReadAllText(htmlFilePath);
            var rssPageFinder = new RssPageFinder(massMedia);
            var valid = rssPageFinder.IsValidRssPage(pageHtml);
            Assert.False(valid);
        }
    }
}
