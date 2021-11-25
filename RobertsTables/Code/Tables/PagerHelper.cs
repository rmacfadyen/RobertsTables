using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RobertsTables.Code.Tables
{
    public static class PagerHelper
    {
        public static IHtmlContent PagedListPageSize(
            this IHtmlHelper htmlHelper,
            X.PagedList.IPagedList records,
            string SortOrder,
            int CurrentPageSize,
            bool LinkUsingJavascript = false
        )
        {
            if (records.PageCount == 0)
            {
                return null;
            }


            var s = new HtmlContentBuilder();
            s.AppendHtml("<div class='d-inline-block'>");
            if (LinkUsingJavascript)
            {
                s.AppendHtml($"<input type=hidden id='PageSize' name='PageSize' value='{CurrentPageSize}' >");
            }

            s.AppendHtml("<span class='pe-1'>Display:</span>");
            s.AppendHtml("<span class='dropdown'>");
            s.AppendHtml("<a class='dropdown-toggle' href='#' role='button' id='dropdownMenuLink' data-bs-toggle='dropdown' aria-expanded='false'>");
            s.Append(CurrentPageSize == -1 ? "All" : CurrentPageSize.ToString());
            s.AppendHtml("</a>");
            s.AppendHtml("<ul class='dropdown-menu dropdown-menu-right ' aria-labelledby='dropdownMenuLink'>");

            PageSizeLink(htmlHelper, s, 10, SortOrder, LinkUsingJavascript);
            PageSizeLink(htmlHelper, s, 25, SortOrder, LinkUsingJavascript);
            PageSizeLink(htmlHelper, s, 50, SortOrder, LinkUsingJavascript);
            PageSizeLink(htmlHelper, s, 100, SortOrder, LinkUsingJavascript);
            PageSizeLink(htmlHelper, s, -1, SortOrder, LinkUsingJavascript);

            s.AppendHtml("</ul></span></div>");

            return s;
        }

        private static void PageSizeLink(
            IHtmlHelper htmlHelper,
            IHtmlContentBuilder s,
            int PageSize,
            string SortOrder,
            bool LinkUsingJavascript = false)
        {
            s.AppendHtml("<li>");
            if (LinkUsingJavascript)
            {
                PageSizeJavascript(s, PageSize);
            }
            else
            {
                PageSizeLinkAsLink(htmlHelper, s, PageSize, SortOrder);
            }
            s.AppendHtml("</li>");
        }



        private static void PageSizeJavascript(
            IHtmlContentBuilder s,
            int PageSize)
        {
            s.AppendHtml($"<a class='dropdown-item' href='javascript:$(\"#PageSize\").val(\"{PageSize}\").closest(\"FORM\")[0].submit();'>{(PageSize == -1 ? "All" : PageSize.ToString())}</a>");
        }

        private static void PageSizeLinkAsLink(
            IHtmlHelper htmlHelper,
            IHtmlContentBuilder s,
            int PageSize,
            string SortOrder)
        {
            s.AppendHtml(htmlHelper.RouteLink(PageSize == -1 ? "All" : PageSize.ToString(), new { SortOrder, PageSize }, new { @class = "dropdown-item w-auto" }));
        }

    }
}