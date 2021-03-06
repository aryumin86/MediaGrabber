﻿using HtmlAgilityPack;
using MediaGrabber.Library.Entities;
using MediaGrabber.Library.Enums;
using MediaGrabber.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaGrabber.Library.ParseRulesIdentifier
{
    /// <summary>
    /// This identifier works with only one Mass Media
    /// </summary>
    public class MassMediaParseRulesIdentifier : IMassMediaParseRulesIdentifier
    {
        private MassMedia _massMedia;
        private readonly int _minimumArticlesNumberWithDescriptionFromRss = 3;
        private HtmlHelper _htmlHelper;
        public MassMediaParseRulesIdentifier(MassMedia massMedia)
        {
            _massMedia = massMedia;
            _htmlHelper = new HtmlHelper();
        }

        /// <summary>
        /// Gets articles whole html pages from rss data.
        /// </summary>
        /// <param name="rssPage"></param>
        /// <returns></returns>
        public override IEnumerable<MayBeArticlePage> GetArticlesFromRssPage(RssPage rssPage)
        {
            var rssReader = new RssReader(_massMedia);
            var articlesBasicData = rssReader.GetArticlesBasicDataFromRssPage(rssPage);
            
            var res = articlesBasicData.Select(x => new MayBeArticlePage(x.Url)
            {
                ProbableBodyPart = x.BodyPart,
                WhenPublished = x.WhenPublished
            });
            
            foreach(var r in res){
                r.BodyHtml = GetPageHtml(r.Url).Result;
                Thread.Sleep(_massMedia.ParsingPauseInMs);
            }

            return res;
        }

        /// <summary>
        /// Tries to find pages with articles using main page of mass media as a start point.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<MayBeArticlePage> GetArticlesFromMainPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to identify single parsing rule of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <returns></returns>
        public override ParsingRule GetMostProbableParsingRule(IEnumerable<RssPage> rssPages)
        {
            //ParsingRule result = null;
            IEnumerable<MayBeArticlePage> mayBeArticles = null;
            IEnumerable<ParsingRule> rules = null;
            if(rssPages != null && rssPages.Any()){
                
                foreach(var rssPage in rssPages)
                {
                    IEnumerable<ParsingRule> rulesFromRssPageArticles = null;
                    mayBeArticles = GetArticlesFromRssPage(rssPage);
                    if(mayBeArticles.Where(a => !string.IsNullOrWhiteSpace(a.ProbableBodyPart)).Count() < _minimumArticlesNumberWithDescriptionFromRss)
                    {
                        rulesFromRssPageArticles = ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithoutDescriptions(rssPage, mayBeArticles);
                    }
                    else
                    {
                        rulesFromRssPageArticles = 
                            ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(rssPage, mayBeArticles);
                    }
                    
                    if(rules == null)
                        rules = new List<ParsingRule>();
                    if(rulesFromRssPageArticles != null && rulesFromRssPageArticles.Any())
                        (rules as List<ParsingRule>).AddRange(rulesFromRssPageArticles);
                    // may be it is a good idea here to try to 
                    // identify parsing rule for THIS rss page (it is foreach for all rss pages)
                }

                //TODO Process rules - find most popular.
                //
                //
                //return result;
                if(rules != null && rules.Any())
                    return rules.First(); // TO here we should identify the most popular rule
            }

            mayBeArticles = GetArticlesFromMainPage();
            rules = ProcessHtmlWithArticlesToIdentifyRulesNotUsingRssPages(mayBeArticles);
            //TODO Process rules - find most popular.
            //
            //
            return rules.FirstOrDefault();
        }

        /// <summary>
        /// Tries to identify parsing rules of mass media using rss pages or (if there are no)
        /// using just mass media pages.
        /// </summary>
        /// <param name="mayBeArticlePages"></param>
        /// <param name="maxParsingRulesNumber"></param>
        /// <returns></returns>
        public override IEnumerable<ParsingRule> GetMostProbableParsingRules(IEnumerable<RssPage> rssPages, int maxParsingRulesNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules using RSS pages data where
        /// RSS item elements have description elements.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithDescriptions(RssPage rssPage, IEnumerable<MayBeArticlePage> articles)
        {
            var result = new List<ParsingRule>();

            // Filtering articles: getting MayBeArticlePage objects collected 
            // from RSS pages which has a description tag
            articles = articles
                .Where(a => !string.IsNullOrWhiteSpace(a.ProbableBodyPart) &&
                    !string.IsNullOrWhiteSpace(a.BodyHtml));

            // If there are at least 'minArticlesWithDescriptin' such articles we use them for 
            // identifying containers which contain article text on article page
            if (articles.Count() < _minimumArticlesNumberWithDescriptionFromRss)
                return null;

            ParsingRule rule = null;
            foreach(var a in articles){
                var allArticleXPathes = new HashSet<string>();
                a.LoadHtml();
                var articleTextNode = FindNodeWithText(a.ProbableBodyPart, a.HtmlDocument);
                if (articleTextNode == null)
                    continue;

                var tempArticleTextNode = articleTextNode;

                var articleTextOnPageContainerTagNameXPath =
                    FindBestTagNameXPathForHtmlNode(articleTextNode, a.HtmlDocument);
                var articleTextOnPageContainerIdXPath =
                    FindBestIdXPathForHtmlNode(articleTextNode, a.HtmlDocument);
                var articleTextOnPageContainerClassXPath =
                    FindBestClassXPathForHtmlNode(articleTextNode, a.HtmlDocument);

                var rudeXPath = tempArticleTextNode.XPath;
                //rule = new ParsingRule()
                //{
                //    XPath = rudeXPath
                //};
                allArticleXPathes.Add(rudeXPath);

                string byIdXpath = null;
                string byTagNameXpath = null;
                string byClassXpath = null;

                tempArticleTextNode = _htmlHelper.MoveAPaceToRootIfAnyOfSiblingsIsText(tempArticleTextNode);

                // moving to root pace by pace trying to identify best rule
                while (byIdXpath != null && byTagNameXpath != null && byClassXpath != null)
                {
                    if (tempArticleTextNode.ParentNode == null)
                        break;

                    if (!string.IsNullOrWhiteSpace(tempArticleTextNode.Id))
                    {
                        byIdXpath = $"//*[@id={tempArticleTextNode.Id}]";
                        allArticleXPathes.Add(byIdXpath);
                    }

                    if(tempArticleTextNode.Attributes["class"] != null)
                    {
                        var classes = tempArticleTextNode.Attributes["class"].Value.Split(" ").ToArray();
                        foreach(var @class in classes)
                        {
                            if(_htmlHelper.ClassIsUniqueForTheWholePage(tempArticleTextNode.OwnerDocument, @class))
                            {
                                byClassXpath = $"//*[contains(@class={@class})]";
                                allArticleXPathes.Add(byClassXpath);
                                break;
                            }
                        }
                    }

                    if(_htmlHelper.NodeNameIsUniqueForWholePage(tempArticleTextNode.OwnerDocument, tempArticleTextNode.Name))
                    {
                        byTagNameXpath = $"//{tempArticleTextNode.Name}";
                        allArticleXPathes.Add(byTagNameXpath);
                    }                        

                    tempArticleTextNode = tempArticleTextNode.ParentNode;
                }

                if (byIdXpath != null)
                    allArticleXPathes.Add(byIdXpath);
                if(byClassXpath != null)
                    allArticleXPathes.Add(byClassXpath);
                if (byTagNameXpath != null)
                    allArticleXPathes.Add(byTagNameXpath);

                if (byIdXpath == null && articleTextOnPageContainerIdXPath != null)
                    allArticleXPathes.Add(articleTextOnPageContainerIdXPath);
                if(byClassXpath == null && articleTextOnPageContainerClassXPath != null)
                    allArticleXPathes.Add(articleTextOnPageContainerIdXPath);
                if (byTagNameXpath == null && articleTextOnPageContainerTagNameXPath != null)
                    allArticleXPathes.Add(articleTextOnPageContainerTagNameXPath);


                //if (!string.IsNullOrWhiteSpace(articleTextOnPageContainerIdXPath)){
                //    rule = new ParsingRule(){
                //        XPath = articleTextOnPageContainerIdXPath
                //    };
                //}
                //if(!string.IsNullOrWhiteSpace(articleTextOnPageContainerClassXPath)){
                //    rule = new ParsingRule(){
                //        XPath = articleTextOnPageContainerClassXPath
                //    };
                //}
                //if(rule != null)
                //    result.Add(rule);

                result.AddRange(allArticleXPathes.Select(r => new ParsingRule()
                {
                    XPath = r
                }));
            }

            return result;
        }

        /// <summary>
        /// Process articles to identify parsing rules without using RSS pages data.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesNotUsingRssPages(IEnumerable<MayBeArticlePage> articles)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process articles to identify parsing rules using RSS pages without description.
        /// </summary>
        /// <param name="articlesPages"></param>
        /// <returns></returns>
        public IEnumerable<ParsingRule> ProcessHtmlWithArticlesToIdentifyRulesUsingRssPagesWithoutDescriptions(RssPage rssPage, IEnumerable<MayBeArticlePage> articles)
        {
            throw new NotImplementedException();
        }

        private HtmlNode FindHtmlNodeWithLongestText(MayBeArticlePage articlePage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens article's url.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public async Task<string> GetPageHtml(string url, 
            WebSiteOpeningType webSiteOpeningType = WebSiteOpeningType.HtmlAgilityPack)
        {
            string res = string.Empty;

            if(webSiteOpeningType == WebSiteOpeningType.HtmlAgilityPack){
                using (var client = new HttpClient())
                {
                    using(var response = await client.GetAsync(url).ConfigureAwait(false))
                    {
                        if(response.StatusCode == HttpStatusCode.Moved)
                        {
                            var redirectUri = response.Headers.Location;
                            if (!redirectUri.IsAbsoluteUri)
                            {
                                redirectUri = new Uri(this._massMedia.MainUrl + redirectUri);
                            }
                            res = GetPageHtml(redirectUri.AbsoluteUri).Result;
                        }
                        else
                        {
                            using (var content = response.Content)
                            {
                                res = await content.ReadAsStringAsync().ConfigureAwait(false);
                            }
                        }                        
                    }
                }
            }
            else{
                throw new NotImplementedException();
            }

            return res;
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string FindBestClassXPathForHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            return _htmlHelper.FindBestClassXPathForHtmlNode(node, doc);
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element id.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string FindBestIdXPathForHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            return _htmlHelper.FindBestIdXPathForHtmlNode(node, doc);
        }

        /// <summary>
        /// Looks for best suited xpath for node with article text using html element tag name.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string FindBestTagNameXPathForHtmlNode(HtmlNode node, HtmlDocument doc)
        {
            return _htmlHelper.FindBestTagNameXPathForHtmlNode(node, doc);
        }

        /// <summary>
        /// Tries to find node of html element where certain text is presented.
        /// It should be unique text coincidences across all nodes.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        private HtmlNode FindNodeWithText(string htmlWithTextToFind, HtmlDocument doc)
        {
            HtmlNode result = null;
            var parsingContext = new ParsingHtmlContext(htmlWithTextToFind);
            _htmlHelper.FindLongestPureTextInHtml(parsingContext);
            if (string.IsNullOrWhiteSpace(parsingContext.LongestTextInHtml))
                return null;
            result = _htmlHelper.LookForUniqueHtmlNodeWithText(parsingContext.LongestTextInHtml, doc);
            
            // trying to take only one longest sentence 
            //if(result == null)
            //{
            //    var longestDescriptionSentence =
            //        parsingContext.LongestTextInHtml.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
            //        .OrderByDescending(x => x.Length)
            //        .First();
            //    result = _htmlHelper.LookForUniqueHtmlNodeWithText(longestDescriptionSentence, doc);
            //}

            //// trying to take only one longest sentence part 
            //if (result == null)
            //{
            //    var longestDescriptionSentence =
            //        parsingContext.LongestTextInHtml.Split(new char[] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries)
            //        .OrderByDescending(x => x.Length)
            //        .First();
            //    result = _htmlHelper.LookForUniqueHtmlNodeWithText(longestDescriptionSentence, doc);
            //}

            return result;
        }
    }
}
