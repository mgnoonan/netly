using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace netly.Helpers
{
    public class Pager
    {
        private ViewContext viewContext;
        private readonly int pageSize;
        private readonly int currentPage;
        private readonly int totalItemCount;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;
        private readonly AjaxOptions ajaxOptions;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary, AjaxOptions ajaxOptions)
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
            this.ajaxOptions = ajaxOptions;
        }

        public HtmlString RenderHtml()
        {
            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            const int numberOfPagesToDisplay = 5;

            var sb = new StringBuilder();

            // Previous
            sb.Append(currentPage > 1 ? GeneratePageLink("<span class=\"page-numbers prev\">prev </span>", currentPage - 1) : "");

            var start = 1;
            var end = pageCount;

            if (pageCount > numberOfPagesToDisplay)
            {
                var middle = (int)Math.Ceiling(numberOfPagesToDisplay / 2d) - 1;
                var below = (currentPage - middle);
                var above = (currentPage + middle);

                if (below < 4)
                {
                    above = numberOfPagesToDisplay;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - numberOfPagesToDisplay);
                }

                start = below;
                end = above;
            }

            if (start >= 2)
            {
                sb.Append(GeneratePageLink("<span class=\"page-numbers\">1</span>", 1));
                //sb.Append(GeneratePageLink("2", 2));
                sb.Append("<span class=\"page-numbers dots\">…</span>");
            }

            for (var i = start; i <= end; i++)
            {
                if (i == currentPage || (currentPage <= 0 && i == 0))
                {
                    sb.AppendFormat("<span class=\"page-numbers current\">{0}</span>", i);
                }
                else
                {
                    var s = string.Format("<span class=\"page-numbers\">{0}</span>", i);
                    sb.Append(GeneratePageLink(s, i));
                }
            }
            if (end <= (pageCount - 2))
            {
                sb.Append("<span class=\"page-numbers dots\">…</span>");
                //sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                //sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
                sb.Append(GeneratePageLink("<span class=\"page-numbers\">" + pageCount + "</span>", pageCount));
            }

            // Next
            sb.Append(currentPage < pageCount ? GeneratePageLink("<span class=\"page-numbers next\"> next</span>", (currentPage + 1)) : "");

            return new HtmlString(sb.ToString());
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(linkWithoutPageValuesDictionary) { { "page", pageNumber }, { "size", pageSize } };
            var virtualPathForArea = RouteTable.Routes.GetVirtualPathForArea(viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathForArea == null)
                return null;

            var stringBuilder = new StringBuilder("<a");

            //if (ajaxOptions != null)
            //    foreach (var ajaxOption in ajaxOptions.ToUnobtrusiveHtmlAttributes())
            //        stringBuilder.AppendFormat(" {0}=\"{1}\"", ajaxOption.Key, ajaxOption.Value);

            stringBuilder.AppendFormat(" href=\"{0}\">{1}</a>", virtualPathForArea.VirtualPath, linkText);

            return stringBuilder.ToString();
        }
    }
}