using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using X.PagedList.Web.Common;

namespace RobertsTables.Code.Tables
{

    public interface ITableHelperService
    {
        string PageLinkWithReturnUrl(string LinkText, string PageName, string ReturnUrl, object IdValues, string LinkCssClass = null);
        string PageLinkWithReturnUrl(string LinkText, string PageName, string SortOrder, int PageSize, int PageNumber, object IdValues, string LinkCssClass = null);
        string PageLink(string LinkText, string PageName, object IdValues, string LinkCssClass = null);

        string PageUrl(string SortOrder, int PageSize, int PageNumber);
        string PageUrl(string SortOrder, int PageSize, int PageNumber, string Letter);
        PagedListRenderOptions PagedListPagerOptions(int PageCount);

        string RowSelectionCheckbox(string Name, string LabelContent, int Value, IList<int> Values);
        string RowSelectionCheckbox(string Name, string LabelContent, string Value, IList<string> Values);
        string RowSelectionCheckbox(string Name, string LabelContent, Guid Value, IList<Guid> Values);

        string RowSelectionRadio(string Name, string LabelContent, string Value, IList<string> Values);
        string RowSelectionRadio(string Name, string LabelContent, int Value, IList<int> Values);
        string PageUrl(string SortOrder, int PageSize, int PageNumber, object IdValues);
    }

    public class TableHelperService : ITableHelperService
    {
        private readonly IHtmlHelper _html;
        private readonly IUrlHelper _url;
        private readonly string _thisPageName;

        public TableHelperService(
            IActionContextAccessor actionContextAccessor,
            IHtmlHelper htmlHelper,
            IUrlHelper urlHelper)
        {
            _html = htmlHelper;
            _url = urlHelper;
            _thisPageName = actionContextAccessor.ActionContext.RouteData.Values["page"] as string;
        }


        public PagedListRenderOptions PagedListPagerOptions(int PageCount)
        {
            return new PagedListRenderOptions
            {
                UlElementClasses = new[] { "pagination" },
                LiElementClasses = new[] { "page-item" },
                PageClasses = new[] { "page-link" },
                ActiveLiElementClass = "active",
                DisplayEllipsesWhenNotShowingAllPageNumbers = false,
                DisplayLinkToFirstPage = PagedListDisplayMode.IfNeeded,
                MaximumPageNumbersToDisplay = 4,
                EllipsesElementClass = "page-link",
                LinkToFirstPageFormat = "1",
                LinkToLastPageFormat = PageCount.ToString(),
                DisplayLinkToPreviousPage = PagedListDisplayMode.Never,
                DisplayLinkToNextPage = PagedListDisplayMode.Never,
            };
        }



        public string PageUrl(string SortOrder, int PageSize, int PageNumber)
        {
            if (PageSize == -1)
            {
                return _url.Page(_thisPageName, new { SortOrder, PageNumber });
            }
            else
            {
                return _url.Page(_thisPageName, new { SortOrder, PageSize, PageNumber });
            }
        }

        public string PageUrl(string SortOrder, int PageSize, int PageNumber, object IdValues)
        {
            IDictionary<string, object> RouteValues;

            RouteValues = new ExpandoObject();
            RouteValues.Add("SortOrder", SortOrder);
            RouteValues.Add("PageNumber", PageNumber);
            if (PageSize != -1)
            {
                RouteValues.Add("PageSize", PageSize);
            }

            foreach (var property in IdValues.GetType().GetProperties())
            {
                RouteValues.Add(property.Name, property.GetValue(IdValues).ToString());
            }

            return _url.Page(_thisPageName, RouteValues);
        }


        public string PageUrl(string SortOrder, int PageSize, int PageNumber, string Letter)
        {
            if (PageSize == -1)
            {
                return _url.Page(_thisPageName, new { SortOrder, PageNumber, Letter });
            }
            else
            {
                return _url.Page(_thisPageName, new { SortOrder, PageSize, PageNumber, Letter });
            }
        }

        public string PageLink(string LinkText, string PageName, object IdValues, string LinkCssClass = null)
        {
            IDictionary<string, object> RouteValues;

            if (!DetermineObjectOrValue(IdValues.GetType()))
            {
                RouteValues = new Dictionary<string, object> {
                    { "id", IdValues }
                };
            }
            else
            {
                RouteValues = new ExpandoObject();

                foreach (var property in IdValues.GetType().GetProperties())
                {
                    RouteValues.Add(property.Name, property.GetValue(IdValues).ToString());
                }
            }

            var EditUrl = _url.Page(PageName, RouteValues);

            if (!string.IsNullOrWhiteSpace(LinkCssClass))
            {
                LinkCssClass = $" class='{LinkCssClass}'";
            }
            var Link = $"<a{LinkCssClass} href=\"{EditUrl}\">{_html.Encode(LinkText)}</a>";

            return Link;
        }



        public string PageLinkWithReturnUrl(
            string LinkText,
            string PageName,
            string ReturnUrl,
            object IdValues,
            string LinkCssClass = null)
        {
            IDictionary<string, object> RouteValues;

            if (!DetermineObjectOrValue(IdValues.GetType()))
            {
                RouteValues = new Dictionary<string, object> {
                    { "id", IdValues }
                };
            }
            else
            {
                RouteValues = new ExpandoObject();

                foreach (var property in IdValues.GetType().GetProperties())
                {
                    var v = property.GetValue(IdValues);
                    if (v is IList<string>)
                    {
                        RouteValues.Add(property.Name, v);
                    }
                    else
                    {
                        RouteValues.Add(property.Name, v.ToString());
                    }
                }
            }

            RouteValues.Add("ReturnToUrl", ReturnUrl);

            var EditUrl = _url.Page(PageName, RouteValues);

            if (!string.IsNullOrWhiteSpace(LinkCssClass))
            {
                LinkCssClass = $" class='{LinkCssClass}'";
            }
            var Link = $"<a{LinkCssClass} href=\"{EditUrl}\">{_html.Encode(LinkText)}</a>";

            return Link;
        }


        public string PageLinkWithReturnUrl(
          string LinkText,
          string PageName,
          string SortOrder,
          int PageSize,
          int PageNumber,
          object IdValues,
          string LinkCssClass = null)
        {
            IDictionary<string, object> RouteValues;

            if (!DetermineObjectOrValue(IdValues.GetType()))
            {
                RouteValues = new Dictionary<string, object> {
                    { "id", IdValues }
                };
            }
            else
            {
                RouteValues = new ExpandoObject();

                foreach (var property in IdValues.GetType().GetProperties())
                {
                    var v = property.GetValue(IdValues);
                    if (v is IList<string>)
                    {
                        RouteValues.Add(property.Name, v);
                    }
                    else
                    {
                        RouteValues.Add(property.Name, v);
                    }
                }
            }

            RouteValues.Add("ReturnToUrl", _url.Page(_thisPageName, new { SortOrder, PageNumber, PageSize }));

            var EditUrl = _url.Page(PageName, RouteValues);

            if (!string.IsNullOrWhiteSpace(LinkCssClass))
            {
                LinkCssClass = $" class='{LinkCssClass}'";
            }
            var Link = $"<a{LinkCssClass} href=\"{EditUrl}\">{_html.Encode(LinkText)}</a>";

            return Link;
        }

        internal static bool DetermineObjectOrValue(Type TypeOfT)
        {
            bool IsObject;

            if (TypeOfT == typeof(string))
            {
                IsObject = false;
            }
            else
            {
                if (!TypeOfT.IsValueType)
                {
                    IsObject = true;
                }
                else
                {
                    if (TypeOfT.IsPrimitive)
                    {
                        IsObject = false;
                    }
                    else
                    {
                        IsObject = true;
                    }
                }
            }

            return IsObject;
        }





        public string RowSelectionCheckbox(
            string Name,
            string LabelContent,
            Guid Value,
            IList<Guid> Values)
        {
            return RowSelectionCheckbox(Name, LabelContent, Value.ToString(), (from v in Values where v.Equals(Value) select 1).Any());
        }

        public string RowSelectionCheckbox(
            string Name,
            string LabelContent,
            int Value,
            IList<int> Values)
        {
            return RowSelectionCheckbox(Name, LabelContent, Value.ToString(), (from v in Values where v.Equals(Value) select 1).Any());
        }

        public string RowSelectionCheckbox(
            string Name,
            string LabelContent,
            string Value,
            IList<string> Values)
        {
            return RowSelectionCheckbox(Name, LabelContent, Value, (from v in Values where v.Equals(Value) select 1).Any());
        }

        public string RowSelectionCheckbox(
           string Name,
           string LabelContent,
           string Value,
           bool Checked)
        {
            var CheckBox =
$@"<div class=""form-check"">
<input type=""checkbox"" class=""form-check-input"" id=""{Name}{_html.Encode(Value)}"" name=""{Name}"" value=""{_html.Encode(Value)}"" {(Checked ? "checked" : "")} >
<label class=""form-check-label"" for=""{Name}{_html.Encode(Value)}"">{LabelContent}</label>
</div>";

            return CheckBox;
        }

        public string RowSelectionRadio(
     string Name,
     string LabelContent,
     string Value,
     IList<string> Values)
        {
            return RowSelectionRadio(Name, LabelContent, Value, (from v in Values where v.Equals(Value) select 1).Any());
        }
        public string RowSelectionRadio(
string Name,
string LabelContent,
int Value,
IList<int> Values)
        {
            return RowSelectionRadio(Name, LabelContent, Value.ToString(), (from v in Values where v.Equals(Value) select 1).Any());
        }


        private string RowSelectionRadio(
         string Name,
         string LabelContent,
         string Value,
         bool Checked)
        {
            var RadioButton =
$@"<div class=""form-check"">
<input type=""radio"" class=""form-check-input"" id=""{Name}{_html.Encode(Value)}"" name=""{Name}"" value=""{_html.Encode(Value)}"" {(Checked ? "checked" : "")} >
<label class=""form-check-label"" for=""{Name}{_html.Encode(Value)}"">{LabelContent}</label>
</div>";

            return RadioButton;
        }
    }
}