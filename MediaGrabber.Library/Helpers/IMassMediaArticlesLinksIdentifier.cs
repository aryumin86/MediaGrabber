using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGrabber.Library.Helpers
{
    /// <summary>
    /// Tries to find links to articles on the main page of mass media. 
    /// </summary>
    public interface IMassMediaArticlesLinksIdentifier
    {
        /// <summary>
        /// Returns all articles links from mass media page.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IEnumerable<string> GetArticlesLinks(string url);
    }
}
