using System;
using System.Collections.Generic;
using System.Text;
using MediaGrabber.Library.Entities;
using Xunit;
using System.Linq;
using MediaGrabber.Library.Helpers;
using System.IO;

namespace MediaGrabber.Library.Tests.Helpers
{
    public class RssReaderTests
    {
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("RssPage_v0.91.xml")]
        [InlineData("RssPage_v0.92.xml")]
        [InlineData("RssPage_v2.0.xml")]
        [InlineData("RssPage_v2.0_2.xml")]
        public void ShouldCollectArticlesFromRss(string fileName)
        {
            var mm = new MassMedia("aaa.ru"){
                Id = 1
            };
            var rssReader = new RssReader(mm);

            var binPath = Environment.CurrentDirectory;
            var xmlRssPageFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "ValidRssPages", fileName);
            var xmlContent = File.ReadAllText(xmlRssPageFilePath);

            var rssPage = new RssPage(){
                 XmlContent = xmlContent
            };
            var result = rssReader.GetArticlesBasicDataFromRssPage(rssPage);

            Assert.True(result.Count() > 0);
            foreach(var a in result){
                Assert.True(a.Url != null);
            }
            Assert.True(result.Any(x => x.BodyPart != null));
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("RssPage_v2.0.xml")]
        [InlineData("RssPage_v2.0_2.xml")]
        public void ShouldCollectArticlesWithOtherDataFromRss(string fileName){
            var mm = new MassMedia("aaa.ru"){
                Id = 1
            };
            var rssReader = new RssReader(mm);

            var binPath = Environment.CurrentDirectory;
            var xmlRssPageFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "ValidRssPages", fileName);
            var xmlContent = File.ReadAllText(xmlRssPageFilePath);

            var rssPage = new RssPage(){
                 XmlContent = xmlContent
            };
            var result = rssReader.GetArticlesBasicDataFromRssPage(rssPage);

            Assert.True(result.Count() > 0);
            foreach(var a in result){
                Assert.True(a.Url != null);
            }
            
            Assert.True(result.Any(x => x.Title != null));
            Assert.True(result.Any(x => x.BodyPart != null));
        }


        [Theory]
        [InlineData("RssPage_v2.0_2.xml")]
        [Trait("Category", "Unit")]
        public void ShouldCollectArticlesWithPubDateFromRss(string fileName){
            var mm = new MassMedia("aaa.ru"){
                Id = 1
            };
            var rssReader = new RssReader(mm);

            var binPath = Environment.CurrentDirectory;
            var xmlRssPageFilePath = 
                Path.Combine(Directory.GetParent(binPath).Parent.Parent.FullName, "TestsData", "ValidRssPages", fileName);
            var xmlContent = File.ReadAllText(xmlRssPageFilePath);

            var rssPage = new RssPage(){
                 XmlContent = xmlContent
            };
            var result = rssReader.GetArticlesBasicDataFromRssPage(rssPage);

            Assert.True(result.Count() > 0);
            foreach(var a in result){
                Assert.True(a.Url != null);
            }

            Assert.True(result.Any(x => x.WhenPublished.Year > 1970));
        }
    }
}
