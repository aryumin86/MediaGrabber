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
        public void ShouldFindRssPagesOnLocalFiles(string htmlFilePath)
        {
            var binPath = Environment.CurrentDirectory;
            htmlFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "RssPages", htmlFilePath);
            var pageHtml = File.ReadAllText(htmlFilePath);
            var rssPageFinder = new RssPageFinder();
            var rssPages = rssPageFinder.ParsePageforRssUrls(pageHtml);
            Assert.True(rssPages.Count() > 0);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("htmlWithoutRssUrls_1.html")]
        public void ShouldNotFindRssPagesOnLocalFiles(string path)
        {
            var pageHtml = File.ReadAllText(path);
            var rssPageFinder = new RssPageFinder();
            var rssPages = rssPageFinder.FindRssPages(pageHtml);
            Assert.True(rssPages.Count() == 0);
        }

        [Theory]
        [Trait("Category", "WebData")]
        [InlineData("abc.ru.html")]
        public void ShouldFindRssPagesOnRemoteWebSite(string url)
        {

        }

        [Theory]
        [Trait("Category", "WebData")]
        [InlineData("ccc.ru.html")]
        public void ShouldNotFindRssPagesOnRemoteWebSite(string url)
        {

        }
    }
}
