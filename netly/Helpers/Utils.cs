using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace netly.Helpers
{
    internal static class Utils
    {

        /// <summary>
        /// Returns a value from the application settings, or an empty string
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <param name="defaultValue">The default value to return if the key does not exist</param>
        /// <returns>
        /// This method checks for the existence of the key in the application settings section of the
        /// web.config, but always returns a string value (never a null). If the key does not exist, 
        /// the default value is returned.
        /// </returns>
        /// 
        public static string GetConfigValueAsString(string key, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
            {
                return defaultValue;
            }
            else
            {
                string value = ConfigurationManager.AppSettings[key].ToString();
                return (value.Length == 0 ? defaultValue : value);
            }
        }

        public static bool GetConfigValueAsBool(string key, bool defaultValue = false)
        {
            string configValue = GetConfigValueAsString(key);

            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            try
            {
                return Convert.ToBoolean(configValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int GetConfigValueAsInt(string key, int defaultValue = 0)
        {
            string configValue = GetConfigValueAsString(key);

            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            try
            {
                return Convert.ToInt32(configValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string stripHtml(string sText)
        {
            String result;

            // Strip out HTML tags
            result = Regex.Replace(sText, "<[^>]*>", " ");

            // Replace HTML constructs
            result = result.Replace("&nbsp;", " ");
            result = result.Replace("&#38;", "&");
            result = result.Replace("&#63;", "?");

            // Replace whitespace
            result = result.Replace("\x09", " ");
            result = result.Replace("\x0A\x0A", "");
            result = result.Replace("\x0A", "\x0D\x0A");
            result = result.Replace("\x0D\x0D", "\x0D");
            result = result.Replace("  ", " ");

            return result;
        }

        public static string stripCrLf(string text, string replacementString = "")
        {
            string pattern = @"[\n\r]";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            return re.Replace(text, replacementString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="metaContentType"></param>
        /// <param name="metaTitle"></param>
        /// <param name="metaKeywords"></param>
        /// <param name="metaDescription"></param>
        /// <returns></returns>
        /// <remarks>
        /// http://codeasp.net/blogs/teisenhauer/microsoft-net/170/parse-meta-tags-in-c-sharp
        /// </remarks>
        public static bool ParseMetaTags(string html, ref string metaContentType, ref string metaTitle,
                                   ref string metaKeywords, ref string metaDescription)
        {
            try
            {
                html = stripCrLf(html);

                // --- Parse the title
                Match m = Regex.Match(html, "<title>([^<]*)</title>", RegexOptions.IgnoreCase);
                metaTitle = m.Groups[1].Value;

                // --- Retrieve all the meta tags into one collection
                var metaTags = Regex.Matches(html, "<meta[^>]+>", RegexOptions.IgnoreCase);
                foreach (Match tag in metaTags)
                {
                    if (Regex.IsMatch(tag.Value, "name=\"?keywords\"?", RegexOptions.IgnoreCase))
                    {
                        m = Regex.Match(tag.Value, "content=\"?([^<]*)\"?", RegexOptions.IgnoreCase);
                        metaKeywords = m.Groups[m.Groups.Count - 1].Value.StripAfter("\"");
                    }
                    if (Regex.IsMatch(tag.Value, "name=\"?description\"?", RegexOptions.IgnoreCase))
                    {
                        m = Regex.Match(tag.Value, "content=\"?([^<]*)\"?", RegexOptions.IgnoreCase);
                        metaDescription = m.Groups[m.Groups.Count - 1].Value.StripAfter("\"");
                    }
                    if (Regex.IsMatch(tag.Value, "name=\"?title\"?", RegexOptions.IgnoreCase) &&
                        string.IsNullOrWhiteSpace(metaTitle))
                    {
                        m = Regex.Match(tag.Value, "content=\"?([^<]*)\"?", RegexOptions.IgnoreCase);
                        metaTitle = m.Groups[m.Groups.Count - 1].Value.StripAfter("\"");
                    }
                    if (Regex.IsMatch(tag.Value, "http-equiv=\"?content-type\"?", RegexOptions.IgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(metaContentType))
                            metaContentType += " | [Meta] ";

                        m = Regex.Match(tag.Value, "content=\"?([^<]*)\"?", RegexOptions.IgnoreCase);
                        metaContentType += m.Groups[m.Groups.Count - 1].Value.StripAfter("\"");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // do something with the error
                return false;
            }
        }

        public static bool ParseHeaders(WebHeaderCollection headers, ref string metaContentType)
        {
            try
            {
                if (headers.AllKeys.Contains("Content-Type"))
                    metaContentType = "[Header] " + headers["Content-Type"];

                return true;
            }
            catch (Exception ex)
            {
                // do something with the error
                return false;
            }
        }

        /// <summary>
        /// Transform the incoming url to an MD5 hash code
        /// </summary>
        /// <param name="url">The url to transform</param>
        /// <returns>A 32 character hash code</returns>
        public static string CreateMD5Hash(string url)
        {
            char[] cs = url.ToLowerInvariant().ToCharArray();
            byte[] buffer = new byte[cs.Length];
            for (int i = 0; i < cs.Length; i++)
                buffer[i] = (byte)cs[i];

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] output = md5.ComputeHash(buffer);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < output.Length; i++)
                builder.AppendFormat("{0:x2}", output[i]);

            return builder.ToString();
        }

        /// <summary>
        /// Convert a long integer to a Base62 string
        /// </summary>
        /// <param name="value">The long integer to convert</param>
        /// <returns>A Base62 string representation of the integer</returns>
        /// <remarks>
        /// Lifted from http://www.shrinkrays.net/articles/friendly-unique-id-generation/part-2.aspx
        /// </remarks>
        public static string Base62ToString(long value)
        {
            // Divides the number by 64, so how many 64s are in  
            // 'value'. This number is stored in Y.   
            // e.g #1   
            // 1) 1000 / 62 = 16, plus 8 remainder (stored in x).  
            // 2) 16 / 62 = 0, remainder 16  
            // 3) 16, 8 or G8:  
            // 4) 65 is A, add 6 to this = 71 or G.  
            //  
            // e.g #2:   
            // 1) 10000 / 62 = 161, remainder 18  
            // 2) 161 / 62 = 2, remainder 37  
            // 3) 2 / 62 = 0, remainder 2  
            // 4) 2, 37, 18, or 2,b,I:  
            // 5) 65 is A, add 27 to this (minus 10 from 37 as these are digits) = 92.  
            //    Add 6 to 92, as 91-96 are symbols. 98 is b.  
            // 6)   
            long x = 0;
            long y = Math.DivRem(value, 62, out x);
            if (y > 0)
                return Base62ToString(y) + ValToChar(x).ToString();
            else
                return ValToChar(x).ToString();
        }

        public static char ValToChar(long value)
        {
            if (value > 9)
            {
                int ascii = (65 + ((int)value - 10));
                if (ascii > 90)
                    ascii += 6;
                return (char)ascii;
            }
            else
                return value.ToString()[0];
        }

        public static long GetSecondsThisCentury()
        {
            DateTime currentDate = DateTime.Now;
            DateTime beginDate = new DateTime(2001, 1, 1);

            long elapsedTicks = currentDate.Ticks - beginDate.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

            return (long)elapsedSpan.TotalSeconds;
        }

        public static long GetTicksThisCentury()
        {
            DateTime currentDate = DateTime.Now;
            DateTime beginDate = new DateTime(2001, 1, 1);

            long elapsedTicks = currentDate.Ticks - beginDate.Ticks;
            //TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

            //return (long)elapsedSpan.TotalMilliseconds;
            return elapsedTicks;
        }

    }
}