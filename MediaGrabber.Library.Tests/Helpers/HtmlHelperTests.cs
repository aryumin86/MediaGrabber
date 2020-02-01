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
        [InlineData("<p>AAaa</p>text<p>aa</p>needed text","needed text")]
        [InlineData("<p>AAaa</p>needed text<p> test","needed text")] //error with tags - no closing one
        [InlineData("<p></p>","")]
        [InlineData("needed text","needed text")] //full string is the longest string
        [InlineData("","")]
        [InlineData("¥","")]
        [InlineData("<![CDATA[<img width=\"150\" height=\"150\" src=\"http://vestnik-lesnoy.ru/wp-content/uploads/2020/01/Плавание-—-копия-150x150.jpg\" class=\"attachment-thumbnail size-thumbnail wp-post-image\" alt=\"\" style=\"float:left; margin:0 10px 10px 0;\" />22 января в плавательном бассейне СШОР «Факел» прошли соревнования по плаванию в зачёт XXI Спартакиады работающей и студенческой молодёжи. В соревнованиях приняло участие 13 команд. В личном первенстве среди женщин на дистанции 25 м вольным стилем места распределились следующим образом: 1 место – Арина Постовалова («Знамя», 14,05 сек.); 2-е – Мария Лещенко («Темп», 15,36 сек.); [&#8230;]]]>", "22 января в плавательном бассейне СШОР «Факел» прошли соревнования по плаванию в зачёт XXI Спартакиады работающей и студенческой молодёжи. В соревнованиях приняло участие 13 команд. В личном первенстве среди женщин на дистанции 25 м вольным стилем места распределились следующим образом: 1 место – Арина Постовалова («Знамя», 14,05 сек.); 2-е – Мария Лещенко («Темп», 15,36 сек.); ")]
        [Trait("Category", "Unit")]
        public void FindLongestPureTextInHtml_Success(string htmlWithText, string textThatShouldBeInLongestTextFragment){
            var parsingContext = new ParsingHtmlContext(htmlWithText){
                FullHtml = htmlWithText
            };
            _htmlHelper.FindLongestPureTextInHtml(parsingContext);
            Assert.Equal(parsingContext.LongestTextInHtml, 
                textThatShouldBeInLongestTextFragment);
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