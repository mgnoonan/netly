//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace netly.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UrlHistoryDetailView
    {
        public int Id { get; set; }
        public string UrlHash { get; set; }
        public string ShortUrl { get; set; }
        public string HttpReferer { get; set; }
        public string HttpUserAgent { get; set; }
        public string RemoteAddr { get; set; }
        public string RemoteHost { get; set; }
        public string ServerName { get; set; }
        public string BaseUri { get; set; }
        public System.DateTime ts { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}
