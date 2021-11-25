using System;

namespace RobertsTables.Code.Tables
{
    public class Column<T>
    {
        public string Caption { get; set; }
        public string CaptionCss { get; set; }
        public string SortBy { get; set; }
        public bool Selectable { get; set; } = false;
        public string CellCss { get; set; }
        public Func<T, string> CaptionHtml { get; set; }
        public Func<T, string> Content { get; set; }
        public Func<T, string> HtmlContent { get; set; }
        public Func<T, string> RowId { get; set; }

        public static Column<T> Create()
        {
            return new Column<T>();
        }
    }
}
