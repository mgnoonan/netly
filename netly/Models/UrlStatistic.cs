using netly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace netly.Models
{
    public class UrlStatistic
    {
        public UrlMap UrlMap { get; set; }
        public UrlAggregate UrlAggregate { get; set; }
        public int Clicks { get; set; }
        public int TotalClicks { get; set; }
        public List<UrlCountryAggregate> Countries { get; set; }
        public string BaseUrl { get; set; }
        public string ShortUrl { get; set; }
        public string AggregateUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}