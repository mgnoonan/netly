using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using netly;
using netly.Helpers;
using System.Net;
using netly.Models;
using Rfc2396Url;

namespace netly.Helpers
{
    public static class UrlShortener
    {
        public static UrlMap Shorten(IRepository repository, string app, string apiKey, string longUrl, string login)
        {
            // Validate incoming parameters
            if (string.IsNullOrEmpty(app))
                throw new ArgumentNullException("Application parameter missing.");
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("ApiKey parameter missing.");
            if (string.IsNullOrEmpty(longUrl))
                throw new ArgumentNullException("Long URL parameter missing.");
            if (string.IsNullOrEmpty(login))
                throw new ArgumentNullException("Login parameter missing.");

            // Convert the ApiKey to a Guid
            Guid apiKeyGuid = new Guid(apiKey);

            var key = repository.FindApiKeys().FirstOrDefault(a => a.ApiKey1 == apiKeyGuid && a.ApplicationName == app);
            if (key == null)
                throw new Exception("Invalid Login or ApiKey. Access denied.");

            Uri target = null;
            string canonicalUrl = null;
            try
            {
                // Get canonicalized url
                var c = new RFC2396Url(longUrl);
                canonicalUrl = c.Canonicalize();

                target = new Uri(longUrl);
            }
            catch
            {
                throw;
            }

            // Calculate hash code for url
            string hash = Utils.CreateMD5Hash(longUrl);

            var agg = repository.FindUrlAggregates().FirstOrDefault(a => a.UrlHash == hash);
            if (agg == null)
            {
                string title = string.Empty;
                string contentType = string.Empty;
                string description = string.Empty;
                string keywords = string.Empty;

                try
                {
                    WebClient client = GetWebClient();
                    string html = client.DownloadString(target);

                    Utils.ParseHeaders(client.ResponseHeaders, ref contentType);
                    Utils.ParseMetaTags(html, ref contentType, ref title, ref keywords, ref description);
                }
                catch (Exception ex)
                {
                    title = "[Unknown]";
                    description = ex.Message;
                }

                agg = new UrlAggregate();
                agg.UrlHash = hash;
                agg.AggregateUrl = Utils.Base62ToString(Utils.GetTicksThisCentury());
                agg.LongUrl = longUrl;
                agg.Title = title.Left(255);
                agg.Description = description.Left(255);
                agg.Keywords = keywords.Left(255);
                agg.ContentType = contentType.Left(100);
                agg.AddedBy = login;
                agg.DateAdded = DateTime.Now;

                repository.AddObject(agg);
                repository.SaveChanges();
            }

            // Write the new short url data to database
            UrlMap map = new UrlMap();
            map.UrlHash = hash;
            map.ShortUrl = GenerateShortUrl(map.Id);
            map.AddedBy = login;
            map.DateAdded = DateTime.Now;

            repository.AddObject(map);
            repository.SaveChanges();

            return map;
        }

        public static UrlMap CustomUrl(IRepository repository, string app, string apiKey, string shortUrl, string customUrl)
        {
            // Check if keyword already in use
            UrlMap map = repository.FindUrlMaps().FirstOrDefault(u => u.CustomUrl == customUrl);
            if (map != null)
            {
                throw new Exception("CustomUrl is already in use.");
            }

            // Check if the hash already exists in the database
            map = repository.FindUrlMaps().FirstOrDefault(u => u.ShortUrl == shortUrl);
            if (map == null)
            {
                throw new Exception("ShortUrl not found.");
            }
            if (!string.IsNullOrEmpty(map.CustomUrl))
            {
                throw new Exception(string.Format("ShortUrl '{0}' already has a CustomUrl.", shortUrl));
            }

            // Add the customUrl to the map
            map.CustomUrl = customUrl;
            repository.SaveChanges();

            return map;
        }

        private static string GenerateShortUrl(int id)
        {
            string generator = Utils.GetConfigValueAsString("ShortUrl.Generator").ToLower();

            switch (generator)
            {
                case "random":
                    return RandomPassword.Generate(5);
                case "DeterministicTicks":
                    return Utils.Base62ToString(Utils.GetTicksThisCentury());
                case "DeterministicSeconds":
                    return Utils.Base62ToString(Utils.GetSecondsThisCentury());
                case "DeterministicID":
                    return Utils.Base62ToString(id);
                default:
                    throw new InvalidOperationException(string.Format("Invalid generator specified '{0}'", generator));
            }
        }

        private static WebClient GetWebClient()
        {
            WebClient client = new WebClient();

            if (ProxyConfig.IsEnabled)
            {
                client.Proxy = new WebProxy(ProxyConfig.ServerName, ProxyConfig.Port);

                if (!string.IsNullOrEmpty(ProxyConfig.Username))
                {
                    var cred = new NetworkCredential(ProxyConfig.Username, ProxyConfig.Password);
                    client.Proxy.Credentials = cred;
                }
            }

            return client;
        }

        private static ProxyServerConfigSettings _proxyConfig = null;
        private static ProxyServerConfigSettings ProxyConfig
        {
            get
            {
                if (_proxyConfig == null)
                {
                    _proxyConfig = new ProxyServerConfigSettings()
                    {
                        IsEnabled = Utils.GetConfigValueAsBool("Proxy.Enabled"),
                        ServerName = Utils.GetConfigValueAsString("Proxy.ServerName"),
                        Port = Utils.GetConfigValueAsInt("Proxy.Port"),
                        Username = Utils.GetConfigValueAsString("Proxy.Username"),
                        Password = Utils.GetConfigValueAsString("Proxy.Password")
                    };
                }

                return _proxyConfig;
            }
        }

    }

    public class ProxyServerConfigSettings
    {
        public bool IsEnabled { get; set; }
        public string ServerName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}