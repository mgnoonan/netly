using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using netly.Helpers;
using netly.Models;

namespace netly.Controllers
{
    public class HomeController : BaseController
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public HomeController() : this(new Repository()) { }
        public HomeController(IRepository repository) { _repository = repository; }

        public ActionResult Index(string url)
        {
            bool displayStats = false;

            // If incoming url is empty, then redirect to the Add action
            if (string.IsNullOrEmpty(url))
            {
                //return RedirectToAction("User", "Info");
                if (Request.IsAuthenticated)
                    return RedirectToAction("Add", "Info");
                else
                    return View();
            }

            // Handle stats display shortcut (append a plus '+' to url hash)
            if (url.EndsWith("+"))
            {
                url = url.Replace("+", "");
                displayStats = true;
            }

            // Lookup url mapping in database
            var hash = (from u in _repository.FindUrlMaps()
                        where u.ShortUrl == url || u.CustomUrl == url
                        select u).SingleOrDefault();

            if (hash != null)
            {
                // Redirect to the stats page, but don't log to history
                if (displayStats)
                    return RedirectToAction("Index", "Info", new { url = url });

                // Write out tracking info to database
                var history = new UrlHistory();
                history.ShortUrl = url;
                history.UrlHash = hash.UrlHash;
                history.HttpReferer = GetServerVariableAsString("HTTP_REFERER").Left(255);
                if (!string.IsNullOrWhiteSpace(history.HttpReferer))
                {
                    Uri uri = new Uri(history.HttpReferer);
                    history.BaseUri = uri.GetLeftPart(UriPartial.Authority);
                }

                history.HttpUserAgent = GetServerVariableAsString("HTTP_USER_AGENT").Left(255);
                history.RemoteAddr = GetServerVariableAsString("REMOTE_ADDR").Left(50);
                history.RemoteHost = GetServerVariableAsString("REMOTE_HOST").Left(50);
                history.ServerName = GetServerVariableAsString("SERVER_NAME").Left(50);
                history.Country = GetCountryCode(history.RemoteAddr).Left(2);
                history.ts = DateTime.Now;
                _repository.AddObject(history);
                _repository.SaveChanges();

                // Retrieve the long url from the aggregate
                var agg = (from a in _repository.FindUrlAggregates() where a.UrlHash == hash.UrlHash select a).SingleOrDefault();

                if (agg == null)
                    throw new HttpException(404, string.Format("Short URL '{0}' was not found.", url));

                // Redirect to new url
                return RedirectPermanent(agg.LongUrl);
            }
            else
            {
                var agg = (from a in _repository.FindUrlAggregates() where a.AggregateUrl == url select a).SingleOrDefault();

                if (agg != null)
                {
                    // Redirect to the stats page, but don't log to history
                    if (displayStats)
                        return RedirectToAction("Index", "Info", new { url = url });

                    // Write out tracking info to database
                    var history = new UrlHistory();
                    history.ShortUrl = agg.AggregateUrl;
                    history.UrlHash = agg.UrlHash;
                    history.HttpReferer = GetServerVariableAsString("HTTP_REFERER");
                    if (!string.IsNullOrWhiteSpace(history.HttpReferer))
                    {
                        Uri uri = new Uri(history.HttpReferer);
                        history.BaseUri = uri.GetLeftPart(UriPartial.Authority);
                    }
                    history.HttpUserAgent = GetServerVariableAsString("HTTP_USER_AGENT");
                    history.RemoteAddr = GetServerVariableAsString("REMOTE_ADDR");
                    history.RemoteHost = GetServerVariableAsString("REMOTE_HOST");
                    history.ServerName = GetServerVariableAsString("SERVER_NAME");
                    history.Country = GetCountryCode(history.RemoteAddr);
                    history.ts = DateTime.Now;
                    _repository.AddObject(history);
                    _repository.SaveChanges();

                    // Redirect to new url
                    return RedirectPermanent(agg.LongUrl);
                }

                // TODO: 404 handler
                throw new HttpException(404, string.Format("Short URL '{0}' was not found.", url));
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }

        private string GetServerVariableAsString(string key)
        {
            try
            {
                string s = HttpContext.Request.ServerVariables[key];

                return string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();
            }
            catch { }

            return string.Empty;
        }

        private string GetCountryCode(string ipAddress)
        {
            string hostIP = "http://api.hostip.info/country.php?ip={0}";

            WebClient client = base.GetWebClient();
            string html = client.DownloadString(string.Format(hostIP, ipAddress)).Trim();

            // Validate that the country code exists in the lookup table
            //var country = new Country();
            var country = (from c in _repository.FindCountries() where c.CountryCode == html select c).SingleOrDefault();
            if (country == null)
            {
                country = new Country();
                country.CountryCode = html;
                country.CountryName = GetCountryName(ipAddress);
                _repository.SaveChanges();
            }

            return html;
        }

        private string GetCountryName(string ipAddress)
        {
            string hostIP = "http://api.hostip.info/get_html.php?ip={0}";

            WebClient client = base.GetWebClient();
            string html = client.DownloadString(string.Format(hostIP, ipAddress)).Trim();

            // Parse the long country name out of the return value
            string pattern = "Country:\\s*(.+)";
            string name = Regex.Match(html, pattern).Groups[1].Value;

            return name;
        }
    }
}