using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobertsTables.Code.Tables
{
    public interface IDateFormatter
    {
        string ToString(DateTimeOffset value);
        string ToString(DateTimeOffset? value);
        string ToStringWithTimeInLocalTime(DateTimeOffset value, bool IncludeSeconds = false);
        string ToStringWithTimeInLocalTime(DateTimeOffset? value, bool IncludeSeconds = false);
        string ToStringTimeOnlyInLocalTime(DateTimeOffset value, bool IncludeSeconds = false);
    }


    public class DateFormatter : IDateFormatter
    {
        TimeZoneInfo tziToronto;

        public DateFormatter()
        {
            tziToronto = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public string ToString(DateTimeOffset value)
        {
            return value.ToString("dd-MMM-yyyy").Replace(".", "");
        }

        public string ToString(DateTimeOffset? value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString("dd-MMM-yyyy").Replace(".", "");
            }

            return "";
        }

        public string ToStringWithTimeInLocalTime(DateTimeOffset value, bool IncludeSeconds = false)
        {
            var LocalTime = TimeZoneInfo.ConvertTime(value, tziToronto);
            return LocalTime.ToString(IncludeSeconds ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy HH:mm").Replace(".", "");
        }


        public string ToStringWithTimeInLocalTime(DateTimeOffset? value, bool IncludeSeconds = false)
        {
            if (value.HasValue)
            {
                var LocalTime = TimeZoneInfo.ConvertTime(value.Value, tziToronto);
                return LocalTime.ToString(IncludeSeconds ? "dd-MMM-yyyy HH:mm:ss" : "dd-MMM-yyyy HH:mm").Replace(".", "");
            }

            return "";
        }

        public string ToStringTimeOnlyInLocalTime(DateTimeOffset value, bool IncludeSeconds = false)
        {
            var LocalTime = TimeZoneInfo.ConvertTime(value, tziToronto);
            return LocalTime.TimeOfDay.ToString(IncludeSeconds ? "HH:mm:ss" : "HH:mm");
        }
    }
}
