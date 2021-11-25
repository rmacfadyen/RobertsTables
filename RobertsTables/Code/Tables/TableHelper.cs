using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace RobertsTables.Code.Tables
{






    public static class TableHelper
    {


        public static IHtmlContent BasicTable<T>(
            this IHtmlHelper htmlHelper,
            string TableId = null,
            string TableCaption = null,
            string NoDataMessage = "No records have been defined.",
            string SortOrder = null,
            string DefaultSortColumn = null,
            string DefaultSortDirection = "asc",
            int? PageSize = null,
            object RouteValues = null,
            IList<string> RouteValuesToExclude = null,
            IEnumerable<T> Data = null,
            IEnumerable<Column<T>> Columns = null,
            IEnumerable<MenuItem> MenuItems = null
        )
        {
            var Content = new StringBuilder();

            TableId ??= "DataList";

            DefaultSortColumn ??= (from c in Columns where !string.IsNullOrWhiteSpace(c.SortBy) select c.SortBy).FirstOrDefault()?.ToLower();
            SortOrder ??= $"{DefaultSortColumn}.{DefaultSortDirection}";
            var (CurrentSortColumn, CurrentAscending) = InitializeSorting(SortOrder, DefaultSortColumn);

            Content.AppendLine($"<table id=\"{TableId}\" cellspacing=\"0\" class=\"table table-hover table-sm table-responsive-lg stickyheaders\">");
            if (!string.IsNullOrWhiteSpace(TableCaption))
            {
                Content.AppendLine($"<caption>{htmlHelper.Encode(TableCaption)}</caption>");
            }
            Content.AppendLine("<thead>");
            var i = 0;
            var LastColumnIndex = Columns.Count();
            foreach (var Column in Columns)
            {
                var ThClass = "";
                if (!string.IsNullOrWhiteSpace(Column.CaptionCss))
                {
                    ThClass = $" class='{Column.CaptionCss}'";
                }
                Content.Append($"<th scope=\"col\"{ThClass}>");

                string ColumnHeading;
                if (string.IsNullOrWhiteSpace(Column.SortBy))
                {
                    ColumnHeading = htmlHelper.Encode(Column.Caption);
                }
                else
                {
                    var asc = "asc";
                    var cssclass = "Sorting";
                    if (Column.SortBy.ToLower() == CurrentSortColumn)
                    {
                        asc = (CurrentAscending ? "desc" : "asc");
                        cssclass += $" Sorted{(CurrentAscending ? "-asc" : "-desc")}";
                    }

                    object defaultRouteValues = new { SortOrder = $"{Column.SortBy}.{asc}", PageSize };
                    object routeValues;
                    if (RouteValues == null)
                    {
                        routeValues = defaultRouteValues;
                    }
                    else
                    {
                        routeValues = ToRouteValues(RouteValues, defaultRouteValues, RouteValuesToExclude);
                    }

                    var Link = htmlHelper.RouteLink(
                        linkText: Column.Caption,
                        htmlAttributes: new { @class = cssclass },
                        routeValues: routeValues
                    );
                    var sw = new StringWriter();
                    Link.WriteTo(sw, System.Text.Encodings.Web.HtmlEncoder.Default);
                    ColumnHeading = sw.ToString();
                }

                if (Column.Selectable)
                {
                    ColumnHeading =
$@"<div class=""form-check"">
<input type=""checkbox"" class=""form-check-input"" id=""{TableId}SelectAll"" name=""{TableId}SelectAll"" >
<label class=""form-check-label"" for=""{TableId}SelectAll"">{ColumnHeading}</label>
</div>";
                }

                Content.Append(ColumnHeading);


                if (MenuItems?.Count() != null)
                {
                    i += 1;
                    if (i == LastColumnIndex)
                    {
                        var Links = new StringBuilder();
                        foreach (var Item in MenuItems)
                        {
                            var Id = Item.Id == null ? string.Empty : $"id='{Item.Id}'";
                            Links.Append($"<a {Id} class='dropdown-item' href='{Item.Url ?? "#"}'>{Item.Caption}</a>");
                        }
                        Content.Append(
$@"<div class='dropdown navbar navbar-light'>
  <button class='navbar-toggler' type='button' id='dropdownMenuButton' data-bs-toggle='dropdown' aria-haspopup='true' aria-expanded='false'>
    <span class='navbar-toggler-icon'></span>
  </button>
  <div class='dropdown-menu dropdown-menu-right' aria-labelledby='dropdownMenuButton'>
    {Links}
  </div>
</div>");
                    }
                }

                Content.AppendLine("</th>");
            }
            Content.AppendLine("</thead>");

            Content.AppendLine("<tbody>");
            if (!Data.Any())
            {
                Content.AppendLine($"<tr><td colspan='{Columns.Count()}' class='PageNoData'>{htmlHelper.Encode(NoDataMessage)}</td><tr>");
            }
            else
            {
                foreach (var Item in Data)
                {
                    Content.AppendLine("<tr>");
                    foreach (var Column in Columns)
                    {
                        string CellContents;
                        if (Column.HtmlContent != null)
                        {
                            CellContents = Column.HtmlContent(Item);
                        }
                        else
                        {
                            CellContents = htmlHelper.Encode(Column.Content(Item));
                        }

                        if (Column.CellCss == null)
                        {
                            Content.Append("<td>");
                        }
                        else
                        {
                            Content.Append($"<td class=\"{Column.CellCss}\">");
                        }

                        Content.Append(CellContents);

                        Content.AppendLine("</td>");
                    }
                    Content.AppendLine("</tr>");
                }
            }
            Content.AppendLine("</tbody>");

            Content.AppendLine("</table>");

            return new HtmlString(Content.ToString());
        }

        public static (string, bool) InitializeSorting(string CurrentSortOrder, string DefaultSortColumn)
        {
            if (string.IsNullOrEmpty(DefaultSortColumn))
            {
                return (null, true);
            }

            var d = DefaultSortColumn.Split('.');
            var defaultSortOrder = (d[0], d.Length == 1 || d[1].ToLower() == "asc");
            if (string.IsNullOrWhiteSpace(CurrentSortOrder))
            {
                return defaultSortOrder;
            }

            var p = CurrentSortOrder.Split('.');
            if (p.Length != 2)
            {
                return defaultSortOrder;
            }

            return (p[0].ToLower(), p[1].ToLower() == "asc");
        }


        internal static IDictionary<string, object> ToRouteValues(
            object Values,
            object AdditionalValues,
            IList<string> PropertiesToExclude = null)
        {
            var expando = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in Values.GetType().GetProperties())
            {
                if (PropertiesToExclude == null || !PropertiesToExclude.Contains(property.Name))
                {
                    expando.Add(property.Name, property.GetValue(Values));
                }
            }

            if (AdditionalValues != null)
            {
                foreach (var property in AdditionalValues.GetType().GetProperties())
                {
                    if (PropertiesToExclude == null || !PropertiesToExclude.Contains(property.Name))
                    {
                        var thisValue = property.GetValue(AdditionalValues);
                        if (!expando.ContainsKey(property.Name))
                        {
                            expando.Add(property.Name, thisValue);
                        }
                        else
                        {
                            expando[property.Name] = thisValue;
                        }
                    }
                }
            }

            return expando;
        }
    }

    /// <summary>
    /// Nullable values are a pain to format. These routines will help.
    /// The rule is a null value formats as an empty string. This works
    /// great for DateTime? values and ok for bool? values.
    /// </summary>
    public static class NullableFormatting
    {
        public static string ToString(this DateTime? Value, string Format)
        {
            var Parts = Format.Split(';');

            if (!Value.HasValue)
            {
                if (Parts.Length > 1)
                {
                    return Parts[1];
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return Value.Value.ToString(Parts[0]);
            }
        }

        public static string ToString(this bool? Value, string Format)
        {
            var Formats = Format.Split(';');
            if (!Value.HasValue)
            {
                if (Format.Length == 3) // "true;false;null"
                {
                    return Formats[2];
                }
                else
                {
                    return "";
                }
            }
            else
            {
                if (Value.Value)
                {
                    return Formats[0];
                }
                else
                {
                    if (Formats.Length > 1)
                    {
                        return Formats[1];
                    }
                    else
                    {
                        return "";
                    }
                }
            }
        }
    }
}
