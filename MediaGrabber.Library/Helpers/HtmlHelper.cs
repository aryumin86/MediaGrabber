using System;
using System.Collections;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Class should be used for html processing like parsing.
    /// </summary>
    public class HtmlHelper
    {
        private string[] _allowedInTextTags = new string[]{
            "p", "a", "img", "b", "strong", "br" //TODO expand this array
        };
        /// <summary>
        /// Identify longest string with text in html without any tags inside
        /// and without any other staff
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string FindLongestPureTextInHtml(string htmlWithText){
            string result = string.Empty;
            int startIndexOfLongestText = 0;
            int endIndexOfLongestText = 0;
            int currentStartIndexOfText = 0;
            Stack<string> tagsStack = new Stack<string>();
            ParseToFindLongestPureText(htmlWithText, out startIndexOfLongestText, 
                out endIndexOfLongestText, currentStartIndexOfText, tagsStack);

            if(tagsStack.Count > 0)
                return null;

            return result;
        }

        /// <summary>
        /// Recursively parses the html with text for each containing tag
        /// end updating start and end indexes of longest text in base html element.
        /// </summary>
        /// <param name="htmlWithText"></param>
        /// <param name="startIndexOfLongestText"></param>
        /// <param name="endIndexOfLongestText"></param>
        /// <param name="currentEndIndexOfText"></param>
        /// <returns>index of tag's end ('>' symbol)</returns>
        private int ParseToFindLongestPureText(string htmlWithText, 
            out int startIndexOfLongestText, out int endIndexOfLongestText, 
            int currentEndIndexOfText, Stack<string> tagsStack){
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to find node with given text in HtmlDocument.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public HtmlNode LookForUniqueHtmlNodeWithText(string text, HtmlDocument doc){
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element id.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string FindBestIdXPathForHtmlNode(HtmlNode node, HtmlDocument doc){
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string FindBestClassXPathForHtmlNode(HtmlNode node, HtmlDocument doc){
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool NodeContainsOnlyTextAndAllowedTags(HtmlNode node){
            throw new NotImplementedException();
        }
    }
}