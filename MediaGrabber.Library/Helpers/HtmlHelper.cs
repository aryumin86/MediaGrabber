using System;
using System.Collections;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        private string[] allowedArticleMainContainerNodeNames = new string[]{
            "span", "div", "article", "section"
        };

        private static char[] wordsSplitChars = new char[] { ' ', ',', ';', '.', '!', '?' };
        private static Regex onlyWordsRegex = new Regex("^[а-яА-Яa-zA-Z0-9]+$", 
            RegexOptions.Compiled | RegexOptions.Singleline);


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
            
            var allNodesWithTextData = new List<HtmlNode>();
            FindAllNodesWithInnerText(doc.DocumentNode,allNodesWithTextData);

            // We will look for same sequence of words in the nodes with text
            var nodesWithThisText = allNodesWithTextData
                .Where(n => HasSameWordsSequences(n.InnerText, textToFind));

            if (nodesWithThisText.Count() > 1)
            {
                result = nodesWithThisText.OrderBy(x => GetNodeChildrenTotalCount(x, 0)).First();
            }
            else
                result = nodesWithThisText.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Defines if two texts has the same words sequences.
        /// </summary>
        /// <param name="text1"></param>
        /// <param name="text2"></param>
        /// <returns></returns>
        public static bool HasSameWordsSequences(string text, string sample, double accuracyRatio = 0.5)
        {
            if (text == null)
                throw new ArgumentException("text can't be null");
            if (sample == null)
                throw new ArgumentException("sample can't be null");

            int sameWordsCount = 0;

            var textWords =
                            text.Split(wordsSplitChars, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => onlyWordsRegex.Match(w))
                            .Select(w => w.Value.ToUpperInvariant())
                            .Where(w => !string.IsNullOrWhiteSpace(w))
                            .ToArray();

            var sampleWords =
                            sample.Split(wordsSplitChars, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => onlyWordsRegex.Match(w))
                            .Select(w => w.Value.ToUpperInvariant())
                            .Where(w => !string.IsNullOrWhiteSpace(w))
                            .ToArray();

            
            
            for (var i = 0; i < textWords.Length; i++)
            {
                for(var j = 0; j < sampleWords.Length; j++)
                {
                    if(textWords[i] == sampleWords[j])
                    {
                        var tempSameWordsCount = 1;
                        var ii = i + 1;
                        var jj = j + 1;
                        while(ii < textWords.Length && jj < sampleWords.Length)
                        {
                            if (textWords[ii] == sampleWords[jj])
                            {
                                tempSameWordsCount++;
                                ii++;
                                jj++;
                            }
                            else
                                break;
                        }

                        if (tempSameWordsCount > sameWordsCount)
                            sameWordsCount = tempSameWordsCount;
                    }
                }
            }

            return sameWordsCount / (double)sampleWords.Length >= accuracyRatio;
        }

        /// <summary>
        /// Recursively gets the number of all children/ subchildren of node.
        /// </summary>
        /// <returns></returns>
        private int GetNodeChildrenTotalCount(HtmlNode node, int count)
        {
            if (node.ChildNodes == null || node.ChildNodes.Count == 0)
                return count;
            return count + node.ChildNodes.Select(n => GetNodeChildrenTotalCount(n, count)).Sum() 
                + node.ChildNodes.Count;
        }

        /// <summary>
        /// Looks for all nodes with text.async Result nodes can contain nodes that contain
        /// other nodes from that list.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="resultList"></param>
        private void FindAllNodesWithInnerText(HtmlNode node, ICollection<HtmlNode> resultList){
            foreach (var child in node.ChildNodes){
                if(string.IsNullOrWhiteSpace(child.InnerText))
                    continue;
                resultList.Add(child);
                if(child.ChildNodes.Any())
                    FindAllNodesWithInnerText(child, resultList);
            }
        }

        /// <summary>
        /// Recursively looks for all nodes that contains only text data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private void FindAllNodesThatContainOnlyTextData(HtmlNode node, ICollection<HtmlNode> nodes)
        {
            foreach (var child in node.ChildNodes)
            {
                if (string.IsNullOrWhiteSpace(child.InnerHtml) || (string.IsNullOrWhiteSpace(child.InnerText)))
                    continue;

                if (NodeContainsOnlyTextAndAllowedTags(child) 
                    && allowedArticleMainContainerNodeNames.Contains(child.Name))
                    nodes.Add(child);
                else if (child.ChildNodes.Any())
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
            string result = null;

            if (node == null)
                throw new ArgumentException("node can't be null");

            if (node.Name == "strong")
                node = node.ParentNode;

            // TODO May be we should consider current current node as needed text container?????
            // In this case we should check current node siblings for being or not text containers???

            var firstParentSutableContainer =
                GetFirstParentNodeWithName(allowedArticleMainContainerNodeNames, node, "id");

            if(firstParentSutableContainer != null)
            {
                result = $"//*[@id={firstParentSutableContainer.Id}]";
            }

            return result;
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public string FindBestClassXPathForHtmlNode(HtmlNode node, HtmlDocument doc){
            string result = null;

            if (node == null)
                throw new ArgumentException("node can't be null");

            if (node.Name == "strong")
                node = node.ParentNode;

            // TODO May be we should consider current current node as needed text container?????
            // In this case we should check current node siblings for being or not text containers???

            // TODO class should be unique for the whole page!!!!!
            HtmlNode firstParentSutableContainer = null;
            while (true) // while not found a unique class for the whole page
            {
                firstParentSutableContainer =
                    GetFirstParentNodeWithName(allowedArticleMainContainerNodeNames, node, "class");
                if (node.Attributes["class"] == null)
                    return result;
                if (ClassIsUniqueForTheWholePage(doc, node.Attributes["class"].Value))
                    break;
                if (node.ParentNode != null)
                {
                    node = node.ParentNode;
                }
                else
                    return result;
            }

            if (firstParentSutableContainer != null)
            {
                result = $"//*[@id={firstParentSutableContainer.Id}]";
            }

            return result;
        }

        /// <summary>
        /// Looks for first parent with one of names in argument neededNames.
        /// </summary>
        /// <param name="neededNames"></param>
        /// <param name="node"></param>
        /// <param name="notNullAttribute">This attribute should be not null</param>
        /// <returns></returns>
        private static HtmlNode GetFirstParentNodeWithName(IEnumerable<string> neededNames, HtmlNode node, string notNullAttribute = null)
        {
            var initNode = node;
            while (true)
            {
                node = node = node.ParentNode;
                if (node == null)
                {
                    node = initNode;
                    break;
                }

                if (neededNames.Contains(node.ParentNode.Name))
                {
                    if(notNullAttribute == null)
                        break;
                    else
                    {
                        if (node.Attributes[notNullAttribute] != null)
                            break;
                    }
                } 
            }
            return node;
        }

        /// <summary>
        /// Identify if a node contains only text data (text and some allowed html tags)
        /// </summary>
        /// <returns></returns>
        private bool NodeContainsOnlyTextAndAllowedTags(HtmlNode node){
            if (node.ChildNodes.All(ch => _allowedInTextTags.Contains(ch.Name)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Is class unique for the whole html page.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private bool ClassIsUniqueForTheWholePage(HtmlDocument doc, string className)
        {
            throw new NotImplementedException();
        }
    }
}