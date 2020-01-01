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
        #region Helpers

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("http://dni.ru", "http://subdomain.dni.ru")]
        [InlineData("http://www.dni.ru", "http://www.subdomain.dni.ru")]
        [InlineData("http://washingtonpost.com", "http://feeds.washingtonpost.com/rss/rss_post-partisan")]
        [InlineData("http://washingtonpost.com", "http://feeds.washingtonpost.com/rss/rss_post-everything")]
        [InlineData("http://washingtonpost.com", "http://feeds.washingtonpost.com/rss/rss_rampage")]
        [InlineData("https://www.washingtonpost.com/", "http://feeds.washingtonpost.com/rss/politics?tid=lk_inline_manual_2")]
        public void IsLocalLink_ShouldReturnTrueIfItIsMassMediaSubdomainLink(string massMediaMainUrl, string massMediaSubDomainUrl)
        {
            var massMedia = new MassMedia(massMediaMainUrl);            
            var rssPageFinder = new RssPageFinder(massMedia);
            var local = rssPageFinder.IsLocalLink(massMediaSubDomainUrl);
            Assert.True(local);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("http://dni.ru", "http://subdomain.nochi.ru")]
        public void IsLocalLink_ShouldReturnFalseIfItIsNotMassMediaSubdomainLink(string massMediaMainUrl, string massMediaSubDomainUrl)
        {
            var massMedia = new MassMedia(massMediaMainUrl);
            var rssPageFinder = new RssPageFinder(massMedia);
            var local = rssPageFinder.IsLocalLink(massMediaSubDomainUrl);
            Assert.False(local);
        }

        #endregion

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("dni.ru.html")]
        [InlineData("rg.ru.html")]
        public void ShouldFindRssPagesOnLocalFiles(string htmlFilePath)
        {
            var massMedia = new MassMedia("http://aaa.ru");
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
            var massMedia = new MassMedia("http://aaa.ru");
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
        [InlineData("https://rg.ru/")]
        [InlineData("https://aif.ru/")]
        [InlineData("https://iz.ru/")]
        [InlineData("https://www.washingtonpost.com/")]
        public void ShouldFindRssPagesOnRemoteWebSite(string url)
        {
            var massMedia = new MassMedia(url)
            {
                Encoding = "utf8",
                Id = 1
            };

            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPages = rssPageFinder.FindRssPages();
            Assert.True(rssPages.Count() > 0);
        }

        [Theory]
        [Trait("Category", "WebData")]
        [InlineData("content-analysis.ru")]
        public void ShouldNotFindRssPagesOnRemoteWebSite(string url)
        {
            var massMedia = new MassMedia(url)
            {
                Encoding = "utf8",
                Id = 1
            };

            var rssPageFinder = new RssPageFinder(massMedia);
            var rssPages = rssPageFinder.FindRssPages();
            Assert.True(rssPages == null);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("RssPage_v0.91.xml")]
        [InlineData("RssPage_v0.92.xml")]
        [InlineData("RssPage_v2.0.xml")]
        [InlineData("RssPage_v2.0_2.xml")]
        public void ShoudIdentifyPageAsValidRss(string htmlFilePath)
        {
            var massMedia = new MassMedia("http://aaa.ru");
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
            var massMedia = new MassMedia("http://aaa.ru");
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
