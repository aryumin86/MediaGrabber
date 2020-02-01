using System;
using System.Collections;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace MediaGrabber.Library.Helpers
{
    public class ParsingHtmlContext {

        public ParsingHtmlContext(string html){
            this.FullHtml = html;
        }

        public string FullHtml {get; set;}
        /// <summary>
        /// Current longest text iteration start index. 
        /// Updates when new iteration starts.
        /// </summary>
        /// <value></value>
        public int StartIndex {get; set;}
        public int CurrentIndex {get; set;}
        public int LongestTextStartIndex {get; set;}
        public int LongestTextEndIndex {get; set;}
        public string LongestTextInHtml {
            get { 
                if (LongestTextEndIndex - LongestTextStartIndex <= 1)
                    return string.Empty;
                else
                    return this.FullHtml.Substring(LongestTextStartIndex, LongestTextEndIndex - LongestTextStartIndex + 1);
            }
        }  
    }

    /// <summary>
    /// Class should be used for html processing like parsing.
    /// </summary>
    public class HtmlHelper
    {
        private HashSet<string> _allowedInTextTags = new HashSet<string>{
            "p", "a", "img", "b", "strong", "br"
        };
        private HashSet<char> allowedSymbolsInText = new HashSet<char>{
            '.', ',', ':', ':', '?', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
            '-', '+', '=', '[', ']', '№'
        };
        private HashSet<char> disAllowedSymbolsInText = new HashSet<char>{
            '[', ']', '\r', '\n', '\t'
        };


        /// <summary>
        /// If there is new longest string discovered -> context should be updated with
        /// new longest string indexes.
        /// </summary>
        /// <value></value>
        private static void UpdateContextLongestStringIfNecessary(ParsingHtmlContext ctx) {
            if(ctx.CurrentIndex - ctx.StartIndex 
                > ctx.LongestTextEndIndex - ctx.LongestTextStartIndex){
                    ctx.LongestTextEndIndex = ctx.CurrentIndex;
                    ctx.LongestTextStartIndex = ctx.StartIndex;
                }
        }

        /// <summary>
        /// Iterate through tag till it ends - till closing sign '>' or end of full html string.
        /// </summary>
        /// <value></value>
        private static void PaceThroughTag(ParsingHtmlContext ctx) {
            var goOn = true;
            while(goOn){
                ctx.CurrentIndex++;
                if(ctx.CurrentIndex >= ctx.FullHtml.Length - 1 
                    || ctx.FullHtml[ctx.CurrentIndex] == '>'){
                        ctx.CurrentIndex++;
                        break;
                    }
            }
        }

        /// <summary>
        /// Looks ahead if there is a closing tag sign '>' ahead.
        /// To tell the truth it is not good way... But I don't care.
        /// </summary>
        /// <value></value>
        private static bool IsThereClosingAngleBracketSignAhead(ParsingHtmlContext ctx){
            for(var i = ctx.CurrentIndex; i < ctx.FullHtml.Length; i++){
                if(ctx.FullHtml[i] == '>') 
                    return true;
            }
            return false;
        }

        /// <summary>
        /// '<' sign (symbol at current index) is a beginning of closing tag.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private static bool IsClosingTagStarts (ParsingHtmlContext ctx) {
            if(ctx.CurrentIndex + 2 >= ctx.FullHtml.Length ||
                ctx.FullHtml[ctx.CurrentIndex] != '/'){
                    return false;
                }
            char currentSimbol;
            for(var i = ctx.CurrentIndex+1; i < ctx.FullHtml.Length; i++){
                currentSimbol = ctx.FullHtml[i];
                if(currentSimbol == '>')
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Identify longest string with text in html without any tags inside
        /// and without any other staff
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public void FindLongestPureTextInHtml(ParsingHtmlContext ctx){
            if (ctx == null)
                throw new ArgumentException("ParsingHtmlContext argument can't be null");
            var goOn = true;
            ctx.StartIndex = ctx.CurrentIndex;
            while(goOn){

                // if it is the end of the whole string - fix string length, break
                if(ctx.CurrentIndex >= ctx.FullHtml.Length - 1){
                    UpdateContextLongestStringIfNecessary(ctx);
                    break;
                }

                var currentSymbol = ctx.FullHtml[ctx.CurrentIndex];

                // if it is a closing tag - fix string length, break.
                if(currentSymbol == '<' && IsClosingTagStarts(ctx)){
                    UpdateContextLongestStringIfNecessary(ctx);
                    PaceThroughTag(ctx);
                    ctx.StartIndex = ctx.CurrentIndex;
                    continue;
                }

                // if it is an opening tag - fix string length, recursively call FindLongestPureTextInHtml().
                if(currentSymbol == '<' && IsThereClosingAngleBracketSignAhead(ctx)){
                    ctx.CurrentIndex -= 1;
                    UpdateContextLongestStringIfNecessary(ctx);
                    ctx.CurrentIndex += 1;
                    PaceThroughTag(ctx);
                    ctx.StartIndex = ctx.CurrentIndex;
                    FindLongestPureTextInHtml(ctx);
                    continue;
                }

                if (disAllowedSymbolsInText.Contains(currentSymbol))
                {
                    ctx.CurrentIndex--;
                    UpdateContextLongestStringIfNecessary(ctx);
                    ctx.CurrentIndex += 2;
                    ctx.StartIndex = ctx.CurrentIndex;
                    continue;
                }

                // if it is a valid text symbol - go on parsing.
                if (char.IsLetter(currentSymbol) 
                    || char.IsNumber(currentSymbol)
                    || char.IsWhiteSpace(currentSymbol)
                    || allowedSymbolsInText.Contains(currentSymbol)){
                    
                }

                ctx.CurrentIndex++;
            }
        }

        /// <summary>
        /// Tries to find node with given text in HtmlDocument.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public HtmlNode LookForUniqueHtmlNodeWithText(string textToFind, HtmlDocument doc){
            HtmlNode result = null;
            if (doc == null)
                throw new ArgumentException("doc argument can't be null");
            if (textToFind == null)
                throw new ArgumentException("textToFind argument can't be null");
            
            var allNodesWithOnlyTextData = new List<HtmlNode>();
            FindAllNodesThatContainOnlyTextData(doc.DocumentNode, allNodesWithOnlyTextData);
            var nodesWithThisText = allNodesWithOnlyTextData
                .Where(n => n.InnerHtml.Contains(textToFind));
            if (nodesWithThisText.Count() > 1)
                return null;

            return result;
        }

        /// <summary>
        /// Recursively looks for all nodes that contains only text data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private void FindAllNodesThatContainOnlyTextData(HtmlNode node, ICollection<HtmlNode> nodes)
        {
            foreach (var child in node.Descendants())
            {
                if (string.IsNullOrWhiteSpace(child.InnerHtml) || (string.IsNullOrWhiteSpace(child.InnerText)))
                    continue;

                if (NodeContainsOnlyTextAndAllowedTags(child))
                    nodes.Add(child);
                else
                    FindAllNodesThatContainOnlyTextData(child, nodes);
            }
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
        /// Identify if a node contains only text data (text and some allowed html tags)
        /// </summary>
        /// <returns></returns>
        public bool NodeContainsOnlyTextAndAllowedTags(HtmlNode node){
            if (node.ChildNodes.All(ch => _allowedInTextTags.Contains(ch.Name)))
                return true;
            else
                return false;
        }
    }
}