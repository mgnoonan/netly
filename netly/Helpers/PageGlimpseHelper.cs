using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using netly.Helpers;

namespace netly.Helpers
{
    public class PageGlimpseHelper
    {
        private static string _baseUrl = "http://images.pageglimpse.com/v1/";
        private static string _baseUrlFormat = "{0}{1}?{2}";
        private static string _apiKey = Utils.GetConfigValueAsString("PageGlimpse.APIKey");

        public static string GetThumbnailUrl(string requestUrl, string size)
        {
            if (Utils.GetConfigValueAsBool("PageGlimpse.Enabled"))
            {
                if (string.IsNullOrEmpty(_apiKey))
                    throw new ArgumentNullException("PageGlimpse.APIKey");

                string queryParams = string.Format("devkey={0}&url={1}&size={2}", _apiKey, HttpUtility.UrlEncode(requestUrl), size);
                string url = string.Format(_baseUrlFormat, _baseUrl, "thumbnails", queryParams);

                return url;
            }
            else
                return "../../content/images/" + 
                    Utils.GetConfigValueAsString("PageGlimpse.DefaultThumbnail");
        }

        //public static string RequestNewThumbnail(string requestUrl)
        //{
        //    if (string.IsNullOrEmpty(_apiKey))
        //        throw new ArgumentNullException("_apiKey");

        //    string queryParams = string.Format("devkey={0}&url={1}", _apiKey, HttpUtility.UrlEncode(requestUrl));
        //    string url = string.Format(_baseUrlFormat, _baseUrl, "thumbnails", queryParams);

        //    WebClient client = new WebClient();
        //    string thumbUrl = client.DownloadString(url);

        //    return thumbUrl;
        //}

        //public static bool ThumbnailExists(string requestUrl, string size)
        //{
        //    if (string.IsNullOrEmpty(_apiKey))
        //        throw new ArgumentNullException("_apiKey");

        //    string queryParams = string.Format("devkey={0}&url={1}&size={2}", _apiKey, HttpUtility.UrlEncode(requestUrl), size);
        //    string url = string.Format(_baseUrlFormat, _baseUrl, "thumbnails", queryParams);

        //    WebClient client = new WebClient();
        //    bool thumbUrlExists = client.DownloadString(url).Contains("\"yes\"");

        //    return thumbUrlExists;
        //}
    }
}