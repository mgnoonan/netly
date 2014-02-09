using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

using netly.Helpers;
using netly.Models;

namespace netly.Controllers
{
    public class InfoController : BaseController
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public InfoController() : this(new Repository()) { }
        public InfoController(IRepository repository) { _repository = repository; }

        //
        // GET: /Info/

        public ActionResult Index(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                // Lookup url mapping in database
                var map = (from u in _repository.FindUrlMaps() where u.ShortUrl == url || u.CustomUrl == url select u).SingleOrDefault();

                if (map != null)
                {
                    var agg = (from a in _repository.FindUrlAggregates() where a.UrlHash == map.UrlHash select a).SingleOrDefault();

                    var stats = new UrlStatistic();
                    stats.UrlMap = map;
                    stats.UrlAggregate = agg;
                    stats.Clicks = (from h in _repository.FindUrlHistories() where h.ShortUrl == map.ShortUrl || h.ShortUrl == map.CustomUrl select h).ToList().Count;
                    stats.TotalClicks = (from h in _repository.FindUrlHistories() where h.UrlHash == map.UrlHash select h).ToList().Count;
                    var countryList = (from h in _repository.FindUrlHistoryDetails() where h.ShortUrl == map.ShortUrl || h.ShortUrl == map.CustomUrl select h).ToList();
                    if (countryList.Count > 0)
                    {
                        stats.Countries = (from c in countryList
                                           group c by c.CountryName into g
                                           select new UrlCountryAggregate { Country = g.Key, Count = g.Count() }).ToList();
                    }
                    else
                    {
                        stats.Countries = new List<UrlCountryAggregate>();
                    }
                    stats.BaseUrl = GetBaseUrl();
                    stats.ShortUrl = GetBaseUrl() + (map.CustomUrl.IsNullOrWhiteSpace() ? map.ShortUrl : map.CustomUrl);
                    stats.AggregateUrl = GetBaseUrl() + agg.AggregateUrl;
                    stats.ThumbnailUrl = PageGlimpseHelper.GetThumbnailUrl(agg.LongUrl, "small");
                    //stats.Conversations = GetTwitterConversations(stats.ShortUrl);

                    return View(stats);
                }
                else
                {
                    var agg = (from a in _repository.FindUrlAggregates() where a.AggregateUrl == url select a).SingleOrDefault();

                    if (agg != null)
                    {
                        var stats = new UrlStatistic();
                        stats.UrlMap = null;
                        stats.UrlAggregate = agg;
                        stats.Clicks = 0;
                        stats.TotalClicks = (from h in _repository.FindUrlHistories() where h.UrlHash == agg.UrlHash select h).ToList().Count;
                        stats.BaseUrl = GetBaseUrl();
                        stats.ShortUrl = GetBaseUrl() + agg.AggregateUrl;
                        stats.AggregateUrl = GetBaseUrl() + agg.AggregateUrl;
                        stats.ThumbnailUrl = PageGlimpseHelper.GetThumbnailUrl(agg.LongUrl, "small");
                        //stats.Conversations = GetTwitterConversations(stats.ShortUrl);
                        var countryList = (from h in _repository.FindUrlHistoryDetails() where h.UrlHash == agg.UrlHash select h).ToList();
                        if (countryList.Count > 0)
                        {
                            stats.Countries = (from c in countryList
                                               group c by c.CountryName into g
                                               select new UrlCountryAggregate { Country = g.Key, Count = g.Count() }).ToList();
                        }
                        else
                        {
                            stats.Countries = new List<UrlCountryAggregate>();
                        }

                        return View(stats);
                    }
                    else
                    {
                        // TODO: 404 handler
                        throw new HttpException(404, string.Format("Short URL '{0}' was not found.", url));
                    }
                }
            }

            return View();
        }

        public ActionResult Analyze()
        {
            return View();
        }

        //private List<TwitterSearchStatus> GetTwitterConversations(string p)
        //{
        //    try
        //    {
        //        if (Utils.GetConfigValueAsBool("Twitter.Enabled"))
        //        {
        //            var service = new TwitterService();
        //            var tweets = service.Search(p);

        //            return new List<TwitterSearchStatus>(tweets.Statuses);

        //            //var twitter = FluentTwitter.CreateRequest().Search().Query().Containing(p);
        //            //var response = twitter.Request();

        //            //return response.AsSearchResult().Statuses;
        //        }
        //    }
        //    catch { }

        //    return new List<TwitterSearchStatus>();
        //}

        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                ModelState.AddModelError("urlMissing", "Please enter a valid URL.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Shorten the requested URL
                    //string executeUrl = string.Format("{0}/Shorten?app='{1}'&apiKey='{2}'&longUrl='{3}'&login='{4}'",
                    //                                    _ctx.BaseUri,
                    //                                    Utils.GetConfigValueAsString("TinyUrl.DataService.AppName"),
                    //                                    Utils.GetConfigValueAsString("TinyUrl.DataService.ApiKey"),
                    //                                    HttpUtility.UrlEncode(url),
                    //                                    string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name) ? "Anonymous" : HttpUtility.UrlEncode(HttpContext.User.Identity.Name));
                    //var map = _ctx.Execute<UrlMap>(new Uri(executeUrl)).SingleOrDefault();
                    var map = UrlShortener.Shorten(_repository, 
                                                    Utils.GetConfigValueAsString("TinyUrl.DataService.AppName"), 
                                                    Utils.GetConfigValueAsString("TinyUrl.DataService.ApiKey"), 
                                                    url, 
                                                    string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name) ? "Anonymous" : HttpUtility.UrlEncode(HttpContext.User.Identity.Name));

                    // Retrieve the aggregate URL
                    var agg = _repository.FindUrlAggregates().FirstOrDefault(a => a.UrlHash == map.UrlHash);

                    // Display the detail information
                    UrlMapDetailViewModel detail = new UrlMapDetailViewModel()
                    {
                        BaseUrl = GetBaseUrl(),
                        FullUrl = GetBaseUrl() + map.ShortUrl,
                        UrlMap = map,
                        UrlAggregate = agg
                    };

                    return View(detail);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex);
                }
            }

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddCustomUrl(string url, string keyword, int id)
        {
            if (string.IsNullOrEmpty(url))
            {
                RedirectToAction("Add");
            }

            try
            {
                // Add a customUrl to the requested shortUrl
                //string executeUrl = string.Format("{0}/CustomUrl?app='{1}'&apiKey='{2}'&shortUrl='{3}'&customUrl='{4}'",
                //                                    _ctx.BaseUri,
                //                                    Utils.GetConfigValueAsString("TinyUrl.DataService.AppName"),
                //                                    Utils.GetConfigValueAsString("TinyUrl.DataService.ApiKey"),
                //                                    HttpUtility.UrlEncode(url),
                //                                    HttpUtility.UrlEncode(keyword));
                //var map = _ctx.Execute<UrlMap>(new Uri(executeUrl)).SingleOrDefault();
                var map = UrlShortener.CustomUrl(_repository,
                                                Utils.GetConfigValueAsString("TinyUrl.DataService.AppName"),
                                                Utils.GetConfigValueAsString("TinyUrl.DataService.ApiKey"),
                                                url,
                                                keyword);

                return RedirectToAction("Index", "Info", new { url = map.ShortUrl });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex);
            }

            return View();
        }

        public new ActionResult User(string username, int? page)
        {
            int pageSize = 15;
            int currentPage = page.HasValue ? page.Value : 1;
            int skipRows = (currentPage - 1) * pageSize;
            
            // Retrieve the list of maps for one username, or all if no username is specified
            IQueryable<UrlMapDetailView> detail = (from d in _repository.FindUrlMapDetails() orderby d.DateAdded descending select d);

            if (!string.IsNullOrEmpty(username))
            {
                detail = detail.Where(d => d.AddedBy == username);
            }
            else
            {
                detail = detail.Take(10);
            }

            // Return the paginated list of the results
            //var list = new PaginatedList<UrlMapDetail>(detail.ToList(), page ?? 0, 10);
            ViewData["username"] = username;
            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = currentPage;
            ViewData["RecordCount"] = detail.AsEnumerable().Count();

            return View(detail.ToPagedList(currentPage - 1, pageSize, (int)ViewData["RecordCount"]));
        }

        private string GetBaseUrl()
        {
            return HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        }
    }
}
