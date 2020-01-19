using Xunit;
using System;
using System.Linq;
using MediaGrabber.Library.Helpers;

namespace MediaGrabber.Library.Tests.Helpers
{
    public class HtmlHelperTests
    {
        private HtmlHelper _htmlHelper;
        public HtmlHelperTests(){
            _htmlHelper = new HtmlHelper();
        }

        [Theory]
        [InlineData("<p>AAaa</p>needed text<p>aa</p> test","needed text")]
        [Trait("Category", "Unit")]
        public void FindLongestPureTextInHtml_Success(string htmlWithText, string textThatShouldBeInLongestTextFragment){
            var foundText = _htmlHelper.FindLongestPureTextInHtml(htmlWithText);
            Assert.True(string.Equals(foundText, 
                textThatShouldBeInLongestTextFragment, StringComparison.OrdinalIgnoreCase));
        }
        
        public void FindLongestPureTextInHtml_FailsWhenNoValidTextInsideNode(){
            Assert.True(false);
        }

        public void LookForUniqueHtmlNodeWithText_Success(){
            Assert.True(false);
        }

        public void LookForUniqueHtmlNodeWithText_FailWhenThereNoOne(){
            Assert.True(false);
        }

        public void LookForUniqueHtmlNodeWithText_FailWhenThereAreMoreThanOneWithThisText(){
            Assert.True(false);
        }

        public void FindBestIdXPathForHtmlNode_Success(){
            Assert.True(false);
        }

        public void FindBestIdXPathForHtmlNode_FailsWhenNoOneWithIdAndOnlyThisTextInside(){
            Assert.True(false);
        }

        public void FindBestClassXPathForHtmlNode_Success(){
            Assert.True(false);
        }

        public void FindBestClassXPathForHtmlNode_FailsWhenNoOneWithUniqueClassesAndOnlyThisTextInside(){
            Assert.True(false);
        }

        public void NodeContainsOnlyTextAndAllowedTags_TrueIfNoTags(){
            Assert.True(false);
        }

        public void NodeContainsOnlyTextAndAllowedTags_TrueIfTextAndAllowedTags(){
            Assert.True(false);
        }

        public void NodeContainsOnlyTextAndAllowedTags_FalseIfNoText(){
            Assert.True(false);
        }

        public void NodeContainsOnlyTextAndAllowedTags_FalseIfContainsOtherTags(){
            Assert.True(false);
        }
    }
}