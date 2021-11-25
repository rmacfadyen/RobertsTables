using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RobertsTables.Code.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace RobertsTables.Pages
{
    public class IndexModel : PageModel
    {
        #region Bind properties
        [BindProperty(SupportsGet = true)] public int[] SelectedRowIds { get; set; }
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; }
        [BindProperty(SupportsGet = true)] public int PageNumber { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
        #endregion


        #region View model
        public IPagedList<ListEntry> RecordsForDisplay { get; set; }
        #endregion

        #region Fake data
        static readonly Random Rnd = new Random();
        static DateTimeOffset RandomTime => DateTime.UtcNow.AddMinutes(-Rnd.Next(1, 3600));

        static readonly IList<ListEntry> Db = new[] {
            new ListEntry { Id = 1, Name = "Robert 1", CreatedOn = RandomTime },
            new ListEntry { Id = 2, Name = "Robert 2", CreatedOn = RandomTime },
            new ListEntry { Id = 3, Name = "Robert 3", CreatedOn = RandomTime },
            new ListEntry { Id = 4, Name = "Robert 4", CreatedOn = RandomTime },
            new ListEntry { Id = 5, Name = "Robert 5", CreatedOn = RandomTime },
            new ListEntry { Id = 6, Name = "Robert 6", CreatedOn = RandomTime },
            new ListEntry { Id = 7, Name = "Robert 7", CreatedOn = RandomTime },
            new ListEntry { Id = 8, Name = "Robert 8", CreatedOn = RandomTime },
            new ListEntry { Id = 9, Name = "Robert 9", CreatedOn = RandomTime },
            new ListEntry { Id = 10, Name = "Robert 10", CreatedOn = RandomTime },
            new ListEntry { Id = 11, Name = "Robert 11", CreatedOn = RandomTime },
            new ListEntry { Id = 12, Name = "Robert 12", CreatedOn = RandomTime },
            new ListEntry { Id = 13, Name = "Robert 13", CreatedOn = RandomTime },
            new ListEntry { Id = 14, Name = "Robert 14", CreatedOn = RandomTime },
            new ListEntry { Id = 15, Name = "Robert 15", CreatedOn = RandomTime },
            new ListEntry { Id = 16, Name = "Robert 16", CreatedOn = RandomTime },
            new ListEntry { Id = 17, Name = "Robert 17", CreatedOn = RandomTime },
            new ListEntry { Id = 18, Name = "Robert 18", CreatedOn = RandomTime },
        };
        #endregion


        public void OnGet()
        { 
            //
            // The query to get the list of records
            //
            var query = (from Record in Db select Record).AsQueryable();

            //
            // Add the sorting
            //
            var (SortByColumn, Ascending) = TableHelper.InitializeSorting(SortOrder, "Id");
            query = SortByColumn.ToLowerInvariant() switch
            {
                "createdon" => query.OrderBy(Ascending, s => s.CreatedOn),
                "name" => query.OrderBy(Ascending, s => s.Name),
                _ => query.OrderBy(Ascending, s => s.Id),
            };

            //
            // Return the list
            //
            RecordsForDisplay = query.ToPagedList(PageNumber, PageSize == -1 ? 9999 : PageSize);
        }

        //
        // What the table will display
        //
        public class ListEntry
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
        }
    }
}
