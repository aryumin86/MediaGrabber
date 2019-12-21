using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Helpers
{
    public interface IMassMediaMainPageRader
    {
        /// <summary>
        /// Returns all articles links from mass media page.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetArticlesLinks(string url);
    }
}
