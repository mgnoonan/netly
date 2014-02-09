using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace netly.Models
{
    public class UrlMapDetailViewModel
    {
        public UrlMap UrlMap { get; set; }
        public UrlAggregate UrlAggregate { get; set; }
        public string BaseUrl { get; set; }
        public string FullUrl { get; set; }
    }
}