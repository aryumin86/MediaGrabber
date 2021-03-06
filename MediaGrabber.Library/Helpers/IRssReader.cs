﻿using MediaGrabber.Library.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Reads RSS xml and gets all articles data from it.
    /// </summary>
    public interface IRssReader
    {
        /// <summary>
        /// Returns all articles links from RSS page.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IEnumerable<Article> GetArticlesBasicDataFromRssPage(RssPage rssPage);

        /// <summary>
        /// Does xml has valid format as RSS page.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        bool IsValidRssPage(string xml);
    }
}
